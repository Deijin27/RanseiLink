#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

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
        RemoveEvolutionCommand = new RelayCommand(RemoveEvolution, () => Evolutions.Count > 0);
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
            case nameof(QuantityForEvolutionCondition1):
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition1Name));
                break;
            case nameof(EvolutionCondition2):
            case nameof(QuantityForEvolutionCondition2):
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition2Name));
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
            var newItem = new EvolutionComboBoxItem(_model.Evolutions, i, _evolutionEntryOptions);
            Evolutions.Add(newItem);
        }
        HabitatItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            HabitatItems.Add(new HabitatItem(model, kingdom, _idToNameService.IdToName<IKingdomService>((int)kingdom)));
        }
        UpdateEvolvesFrom();
        RaiseAllPropertiesChanged();
        RemoveEvolutionCommand.RaiseCanExecuteChanged();
    }

    public ICommand ImportAnimationCommand { get; }
    public RelayCommand RevertAnimationCommand { get; }
    public ICommand ExportAnimationsCommand { get; }
    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public string QuantityForEvolutionCondition1Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition1, _model.QuantityForEvolutionCondition1);

    private string GetNameOfQuantityForEvolutionCondition(EvolutionConditionId id, int quantity)
    {
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

    public string QuantityForEvolutionCondition2Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition2, _model.QuantityForEvolutionCondition2);

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
        public EvolutionComboBoxItem(List<PokemonId> evolutionTable, int id, List<SelectorComboBoxItem> options)
        {
            _id = id;
            _evolutionTable = evolutionTable;
            Options = options;
        }
        public int Id
        {
            get => (int)_evolutionTable[_id];
            set => SetProperty(Id, value, v => _evolutionTable[_id] = (PokemonId)v);
        }
        public List<SelectorComboBoxItem> Options { get; }
    }

    public ObservableCollection<EvolutionComboBoxItem> Evolutions { get; } = new();

    public ICommand AddEvolutionCommand { get; }
    public RelayCommand RemoveEvolutionCommand { get; }

    private void AddEvolution()
    {
        _model.Evolutions.Add(PokemonId.Eevee);
        var newItem = new EvolutionComboBoxItem(_model.Evolutions, _model.Evolutions.Count - 1, _evolutionEntryOptions);
        Evolutions.Add(newItem);
        RemoveEvolutionCommand.RaiseCanExecuteChanged();
    }

    private void RemoveEvolution()
    {
        if (_model.Evolutions.Count == 0)
        {
            return;
        }
        _model.Evolutions.RemoveAt(_model.Evolutions.Count - 1);
        Evolutions.RemoveAt(Evolutions.Count - 1);
        RemoveEvolutionCommand.RaiseCanExecuteChanged();
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
            InitialFileName = $"{(int)_id}_{_model.Name}_NSPAT.xml",
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
