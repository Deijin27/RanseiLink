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
    private readonly IItemService _itemService;
    private readonly IAnimGuiManager _animGuiManager;
    private readonly ICommand _selectItemCommand;

    public KingdomViewModel(ICachedSpriteProvider cachedSpriteProvider, IItemService itemService, INicknameService nicknameService, IJumpService jumpService, IIdToNameService idToNameService, IAnimGuiManager animGuiManager)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _itemService = itemService;
        _animGuiManager = animGuiManager;
        JumpToBattleConfigCommand = new RelayCommand<int>(id => jumpService.JumpTo(BattleConfigWorkspaceEditorModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonWorkspaceModule.Id, id));
        _selectItemCommand = new RelayCommand<ItemMiniViewModel>(miniVm => { if (miniVm != null) jumpService.JumpTo(ItemWorkspaceModule.Id, miniVm.Id); });
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        BattleConfigItems = nicknameService.GetAllNicknames(nameof(BattleConfigId));
        PokemonItems = idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        PurchasableItems = [];
        foreach (var method in Enum.GetValues<PurchaseMethodId>().Where(x => x != PurchaseMethodId.WanderingMerchant))
        {
            var group = new PurchasableItemGroup(method, GetPurchaseMethodName(method));
            PurchasableItems.Add(group);
        }
    }

    public void SetModel(KingdomId id, Kingdom model)
    {
        _id = id;
        _model = model;
        KingdomImageAnimVm = new(_animGuiManager, AnimationTypeId.KuniImage2, (int)id);
        CastlemapAnimVm = new(_animGuiManager, AnimationTypeId.Castlemap, (int)id);
        KingdomIconAnimVm = new(_animGuiManager, AnimationTypeId.IconCastle, (int)id);
        UpdatePurchasable();
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

    private void UpdatePurchasable()
    {
        foreach (var group in PurchasableItems)
        {
            group.Items.ResetTo(GetPurchasableItems(group.Method));
        }
    }

    private static int _selectedPurchasableGroup;
    public int SelectedPurchasableGroup
    {
        get => _selectedPurchasableGroup;
        set => SetProperty(ref _selectedPurchasableGroup, value);
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

    public class PurchasableItemGroup(PurchaseMethodId method, string title)
    {
        public PurchaseMethodId Method { get; } = method;
        public string Title { get; } = title;
        public ObservableCollection<ItemMiniViewModel> Items { get; } = [];
    }
}
