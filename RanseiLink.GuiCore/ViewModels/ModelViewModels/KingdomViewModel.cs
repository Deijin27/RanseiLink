#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class KingdomViewModel : ViewModelBase, IBigViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly IPokemonService _pokemonService;
    private readonly IItemService _itemService;
    private readonly IAnimGuiManager _animGuiManager;
    private readonly ICommand _selectItemCommand;
    private readonly ICommand _selectPokemonCommand;

    public KingdomViewModel(ICachedSpriteProvider cachedSpriteProvider, IPokemonService pokemonService, IItemService itemService, INicknameService nicknameService, IJumpService jumpService, IIdToNameService idToNameService, IAnimGuiManager animGuiManager)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _pokemonService = pokemonService;
        _itemService = itemService;
        _animGuiManager = animGuiManager;
        JumpToBattleConfigCommand = new RelayCommand<int>(id => jumpService.JumpTo(BattleConfigWorkspaceEditorModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonWorkspaceModule.Id, id));
        _selectItemCommand = new RelayCommand<ItemMiniViewModel>(miniVm => { if (miniVm != null) jumpService.JumpTo(ItemWorkspaceModule.Id, miniVm.Id); });
        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(miniVm => { if (miniVm != null) jumpService.JumpTo(PokemonWorkspaceModule.Id, miniVm.Id); });
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        BattleConfigItems = nicknameService.GetAllNicknames(nameof(BattleConfigId));
        PokemonItems = idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        PurchasableItems = [];
        foreach (var method in Enum.GetValues<PurchaseMethodId>().Where(x => x != PurchaseMethodId.WanderingMerchant))
        {
            var group = new PurchasableItemGroup(method, GetPurchaseMethodName(method));
            PurchasableItems.Add(group);
        }

        PokemonGroups = [
            new EncounterablePokemonGroup(false, "Pokemon"),
            new EncounterablePokemonGroup(true, "Pokemon Lv2 Area")
            ];
        
    }

    public void SetModel(KingdomId id, Kingdom model)
    {
        _id = id;
        _model = model;
        KingdomImageAnimVm = new(_animGuiManager, AnimationTypeId.KuniImage2, (int)id);
        CastlemapAnimVm = new(_animGuiManager, AnimationTypeId.Castlemap, (int)id);
        KingdomIconAnimVm = new(_animGuiManager, AnimationTypeId.IconCastle, (int)id);
        UpdatePurchasable();
        UpdatePokemon();
        RaiseAllPropertiesChanged();
    }

    public void SetModel(int id, object model)
    {
        SetModel((KingdomId)id, (Kingdom)model);
    }

    public AnimationViewModel? KingdomImageAnimVm { get; private set; }
    public AnimationViewModel? CastlemapAnimVm { get; private set; }
    public AnimationViewModel? KingdomIconAnimVm { get; private set; }

    public ICommand JumpToBattleConfigCommand { get; }
    public ICommand JumpToPokemonCommand { get; }

    public List<PurchasableItemGroup> PurchasableItems { get; }
    public List<EncounterablePokemonGroup> PokemonGroups { get; }

    private void UpdatePurchasable()
    {
        foreach (var group in PurchasableItems)
        {
            group.Items.ResetTo(GetPurchasableItems(group.Method));
        }
    }

    private void UpdatePokemon()
    {
        foreach (var group in PokemonGroups)
        {
            group.Pokemon.ResetTo(GetEncounterablePokemon(group.RequiresLv2));
        }
    }

    private static int __selectedPurchasableGroup;
    public int SelectedPurchasableGroup
    {
        get => __selectedPurchasableGroup;
        set => SetProperty(ref __selectedPurchasableGroup, value);
    }

    private static int __selectedPokemonGroup;
    public int SelectedPokemonGroup
    {
        get => __selectedPokemonGroup;
        set => SetProperty(ref __selectedPokemonGroup, value);
    }

    private static string GetPurchaseMethodName(PurchaseMethodId id)
    {
        return id switch
        {
            PurchaseMethodId.BuildingLevel0 => "Shop Lv0",
            PurchaseMethodId.BuildingLevel1 => "Shop Lv1",
            PurchaseMethodId.BuildingLevel2 => "Show Lv2",
            PurchaseMethodId.BuildingLevel3 => "Shop Lv 3",
            _ => "",
        };
    }

    public List<ItemMiniViewModel> GetPurchasableItems(PurchaseMethodId method)
    {
        var list = new List<ItemMiniViewModel>();
        foreach (var id in _itemService.ValidIds())
        {
            var item = _itemService.Retrieve(id);
            if (item.GetPurchasable(_id) && item.PurchaseMethod == method)
            {
                list.Add(new ItemMiniViewModel(_cachedSpriteProvider, item, id, _selectItemCommand));
            }
        }
        return list;
    }

    public List<PokemonMiniViewModel> GetEncounterablePokemon(bool requiresLv2)
    {
        var list = new List<PokemonMiniViewModel>();
        foreach (var id in _pokemonService.ValidIds())
        {
            var pokemon = _pokemonService.Retrieve(id);
            if (pokemon.GetEncounterable(_id, requiresLv2))
            {
                list.Add(new PokemonMiniViewModel(_cachedSpriteProvider, pokemon, id, _selectPokemonCommand));
            }
        }
        return list;
    }

    public class PurchasableItemGroup(PurchaseMethodId method, string title)
    {
        public PurchaseMethodId Method { get; } = method;
        public string Title { get; } = title;
        public ObservableCollection<ItemMiniViewModel> Items { get; } = [];
    }

    public class EncounterablePokemonGroup(bool requiresLv2, string title)
    {
        public bool RequiresLv2 { get; } = requiresLv2;
        public string Title { get; } = title;
        public ObservableCollection<PokemonMiniViewModel> Pokemon { get; } = [];
    }
}
