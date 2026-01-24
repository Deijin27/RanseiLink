#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace RanseiLink.GuiCore.ViewModels;

public partial class PokemonViewModel : ViewModelBase, IBigViewModel
{
    private readonly List<SelectorComboBoxItem> _evolutionEntryOptions;
    private readonly IIdToNameService _idToNameService;
    private readonly IKingdomService _kingdomService;
    private readonly IItemService _itemService;
    private readonly IPokemonService _pokemonService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IAsyncDialogService _dialogService;
    private readonly IPokemonAnimationService _animationService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    private readonly ICommand _selectPokemonCommand;
    public PokemonViewModel(
        IJumpService jumpService, 
        IIdToNameService idToNameService, 
        IKingdomService kingdomService, 
        IItemService itemService, 
        IPokemonService pokemonService,
        IOverrideDataProvider spriteProvider, 
        SpriteItemViewModel.Factory spriteItemVmFactory, 
        IAsyncDialogService dialogService, 
        IPokemonAnimationService animationService,
        ICachedSpriteProvider cachedSpriteProvider)
    {
        _animationService = animationService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _spriteItemVmFactory = spriteItemVmFactory;
        _idToNameService = idToNameService;
        _kingdomService = kingdomService;
        _itemService = itemService;
        _pokemonService = pokemonService;
        _spriteProvider = spriteProvider;
        _dialogService = dialogService;

        MoveItems = _idToNameService.GetComboBoxItemsExceptDefault<IMoveService>();
        AbilityItems = _idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();

        _evolutionEntryOptions = _idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        JumpToMoveCommand = new RelayCommand<int>(id => jumpService.JumpTo(MoveWorkspaceModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilityWorkspaceEditorModule.Id, id));
        AddEvolutionCommand = new RelayCommand(AddEvolution);
        ViewSpritesCommand = new RelayCommand(ViewSprites);
        ImportAnimationCommand = new RelayCommand(ImportAnimation);
        ExportAnimationsCommand = new RelayCommand(ExportAnimations);
        RevertAnimationCommand = new RelayCommand(RevertAnimation, () => IsAnimationOverwritten);
        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) jumpService.JumpTo(PokemonWorkspaceModule.Id, pk.Id); });


        PropertyChanged += PokemonViewModel_PropertyChanged;
    }

    private void PokemonViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(EvolutionCondition1):
                RaisePropertyChanged(nameof(EvolutionCondition1_Quantity1_Name));
                RaisePropertyChanged(nameof(EvolutionCondition1_Quantity2_Name));
                break;
            case nameof(EvolutionCondition1_Quantity1):
                RaisePropertyChanged(nameof(EvolutionCondition1_Quantity1_Name));
                break;
            case nameof(EvolutionCondition1_Quantity2):
                RaisePropertyChanged(nameof(EvolutionCondition1_Quantity2_Name));
                break;
            case nameof(EvolutionCondition2):
                RaisePropertyChanged(nameof(EvolutionCondition2_Quantity1_Name));
                RaisePropertyChanged(nameof(EvolutionCondition2_Quantity2_Name));
                break;
            case nameof(EvolutionCondition2_Quantity1):
                RaisePropertyChanged(nameof(EvolutionCondition2_Quantity1_Name));
                break;
            case nameof(EvolutionCondition2_Quantity2):
                RaisePropertyChanged(nameof(EvolutionCondition2_Quantity2_Name));
                break;
        }
    }

    public void SetModel(int id, object model)
    {
        SetModel((PokemonId)id, (Pokemon)model);
    }

    public void SetModel(PokemonId id, Pokemon model)
    {
        _model = model;
        _id = id;
        Evolutions.Clear();
        for (int i = 0; i < _model.Evolutions.Count; i++)
        {
            AddEvolutionToUi(i);
        }
        HabitatItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            HabitatItems.Add(new HabitatItem(model, kingdom, _idToNameService.IdToName<IKingdomService>((int)kingdom)));
        }
        UpdateEvolvesFrom();
        RaiseAllPropertiesChanged();
    }

    public ICommand ImportAnimationCommand { get; }
    public RelayCommand RevertAnimationCommand { get; }
    public ICommand ExportAnimationsCommand { get; }
    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public string EvolutionCondition1_Quantity1_Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition1, _model.EvolutionCondition1_Quantity1);
    public string EvolutionCondition1_Quantity2_Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition1, _model.EvolutionCondition1_Quantity2);

    public string EvolutionCondition2_Quantity1_Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition2, _model.EvolutionCondition2_Quantity1);
    public string EvolutionCondition2_Quantity2_Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition2, _model.EvolutionCondition2_Quantity2);

    private string GetNameOfQuantityForEvolutionCondition(EvolutionConditionId id, int quantity)
    {
        if (quantity == 511)
        {
            return "Default";
        }
        switch (id)
        {
            case EvolutionConditionId.Kingdom:
                if (_kingdomService.ValidateId(quantity))
                {
                    return _kingdomService.IdToName(quantity);
                }
                return "Invalid";

            case EvolutionConditionId.WarriorGender:
                return $"{(GenderId)quantity}";

            case EvolutionConditionId.Item:
                if (_itemService.ValidateId(quantity))
                {
                    return _itemService.IdToName(quantity);
                }
                return "Invalid";
            default:
                return "";
        }
    }

    public ObservableCollection<HabitatItem> HabitatItems { get; } = [];

    public class HabitatItem : ViewModelBase
    {
        private readonly KingdomId _kingdom;
        private readonly Pokemon _model;
        public string KingdomName { get; }

        public HabitatItem(Pokemon pokemon, KingdomId kingdom, string kingdomName)
        {
            _kingdom = kingdom;
            _model = pokemon;
            KingdomName = kingdomName;
        }

        public bool EncounterableAtDefaultArea
        {
            get => _model.GetEncounterable(_kingdom, false);
            set => SetProperty(EncounterableAtDefaultArea, value, v => _model.SetEncounterable(_kingdom, false, v));
        }

        public bool EncounterableWithLevel2Area
        {
            get => _model.GetEncounterable(_kingdom, true);
            set => SetProperty(EncounterableWithLevel2Area, value, v => _model.SetEncounterable(_kingdom, true, v));
        }
    }

    public class EvolutionComboBoxItem : ViewModelBase
    {
        private readonly List<PokemonId> _evolutionTable;
        private readonly int _id;
        private readonly Func<int, PokemonMiniViewModel> _createPokemonMiniVm;
        public EvolutionComboBoxItem(List<PokemonId> evolutionTable, int id, List<SelectorComboBoxItem> options, 
            Func<int, PokemonMiniViewModel> createPokemonMiniVm)
        {
            _id = id;
            _evolutionTable = evolutionTable;
            _createPokemonMiniVm = createPokemonMiniVm;
            UpdateMiniVm();
            Options = options;
            
            DeleteCommand = new RelayCommand(Delete);
        }

        [MemberNotNull(nameof(MiniViewModel))]
        private void UpdateMiniVm()
        {
            MiniViewModel = _createPokemonMiniVm(Id);
            RaisePropertyChanged(nameof(MiniViewModel));
        }

        private void Delete()
        {
            RequestRemove?.Invoke(this, _id);
        }

        public event EventHandler<int>? RequestRemove;

        public int Id
        {
            get => (int)_evolutionTable[_id];
            set
            {
                if (SetProperty(Id, value, v => _evolutionTable[_id] = (PokemonId)v))
                {
                    UpdateMiniVm();
                }
            }
        }

        public PokemonMiniViewModel MiniViewModel { get; private set; }

        public List<SelectorComboBoxItem> Options { get; }

        public ICommand DeleteCommand { get; }
    }

    public ObservableCollection<EvolutionComboBoxItem> Evolutions { get; } = [];

    public ICommand AddEvolutionCommand { get; }

    private void AddEvolutionToUi(int id)
    {
        var newItem = new EvolutionComboBoxItem(_model.Evolutions, id, _evolutionEntryOptions, 
            id =>
            {
                var pokemon = _pokemonService.Retrieve(id);
                return new PokemonMiniViewModel(_cachedSpriteProvider, pokemon, id, _selectPokemonCommand);
            });
        newItem.RequestRemove += EvolutionItem_RequestRemove;
        Evolutions.Add(newItem);
    }

    private void EvolutionItem_RequestRemove(object? sender, int id)
    {
        _model.Evolutions.RemoveAt(id);
        Evolutions.RemoveAt(id);
    }

    private void AddEvolution()
    {
        _model.Evolutions.Add(PokemonId.Eevee);
        AddEvolutionToUi(_model.Evolutions.Count - 1);
    }

    public ObservableCollection<PokemonMiniViewModel> EvolvesFrom { get; } = [];

    private void UpdateEvolvesFrom()
    {
        EvolvesFrom.Clear();
        foreach (var id in _pokemonService.ValidIds())
        {
            var pokemon = _pokemonService.Retrieve(id);
            if (pokemon.Evolutions.Contains(_id))
            {
                EvolvesFrom.Add(new PokemonMiniViewModel(_cachedSpriteProvider, pokemon, id, _selectPokemonCommand));
            }
        }
    }

    public string SmallSpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlPokemonM, (int)_id).File;

    public ICommand ViewSpritesCommand { get; }
    private void ViewSprites()
    {
        int id = (int)_id;
        List<SpriteFile> sprites =
        [
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonB, id),
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonCI, id),
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonL, id),
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonM, id),
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonS, id),
            _spriteProvider.GetSpriteFile(SpriteType.StlPokemonSR, id),
            //_spriteProvider.GetSpriteFile(SpriteType.StlPokemonWu, id);
            _spriteProvider.GetSpriteFile(SpriteType.ModelPokemon, id),
        ];

        var vm = new ImageListViewModel(sprites, _spriteItemVmFactory);
        _dialogService.ShowDialog(vm);

        RaisePropertyChanged(nameof(SmallSpritePath));

    }

    private async Task ImportAnimation()
    {
        var file = await _dialogService.ShowOpenSingleFileDialog(new OpenFileDialogSettings
        {
            Title = "Select the raw pattern animation library file",
            Filters =
            [
                new()
                {
                    Name = "Pattern Animation XML (.xml)",
                    Extensions = [".xml"]
                }
            ]
        });

        if (string.IsNullOrEmpty(file))
        {
            return;
        }
        var result = _animationService.ImportAnimation(_id, file);
        if (result.IsFailed)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to export animation", result.ToString()));
            return;
        }

        RaisePropertyChanged(nameof(IsAnimationOverwritten));
        RaisePropertyChanged(nameof(LongAttackAnimation));
        RaisePropertyChanged(nameof(AsymmetricBattleSprite));
        RevertAnimationCommand.RaiseCanExecuteChanged();
    }

    private async Task ExportAnimations()
    {
        if (!_spriteProvider.IsDefaultsPopulated())
        {
            return;
        }

        var dest = await _dialogService.ShowSaveFileDialog(new()
        {
            Title = "Export animation file",
            DefaultExtension = ".xml",
            SuggestedFileName = $"{(int)_id}_{_model.Name}_NSPAT.xml",
            Filters =
            [
                new()
                {
                    Name = "Pattern Animation XML (.xml)",
                    Extensions = [".xml"]
                }
            ]
        });

        if (string.IsNullOrEmpty(dest))
        {
            return;
        }

        var result = _animationService.ExportAnimations(_id, dest);
        if (result.IsFailed)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to export animation", result.ToString()));
        }
    }

    private void RevertAnimation()
    {
        _animationService.RevertAnimation(_id);
        RaisePropertyChanged(nameof(IsAnimationOverwritten));
        RevertAnimationCommand.RaiseCanExecuteChanged();
    }

    public bool IsAnimationOverwritten => _animationService.IsAnimationOverwritten(_id);
}
