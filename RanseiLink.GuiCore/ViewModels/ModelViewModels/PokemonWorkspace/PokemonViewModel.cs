#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class PokemonViewModel : ViewModelBase
{
    private Pokemon _model;
    private readonly List<SelectorComboBoxItem> _evolutionEntryOptions;
    private readonly IIdToNameService _idToNameService;
    private readonly IKingdomService _kingdomService;
    private readonly IItemService _itemService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IAsyncDialogService _dialogService;
    private readonly IPokemonAnimationService _animationService;
    private PokemonId _id;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    public PokemonViewModel(
        IJumpService jumpService, 
        IIdToNameService idToNameService, 
        IKingdomService kingdomService, 
        IItemService itemService, 
        IOverrideDataProvider spriteProvider, 
        SpriteItemViewModel.Factory spriteItemVmFactory, 
        IAsyncDialogService dialogService, 
        IPokemonAnimationService animationService,
        CopyPasteViewModel copyPasteVm)
    {
        _animationService = animationService;
        CopyPasteVm = copyPasteVm;
        _spriteItemVmFactory = spriteItemVmFactory;
        _idToNameService = idToNameService;
        _kingdomService = kingdomService;
        _itemService = itemService;
        _spriteProvider = spriteProvider;
        _dialogService = dialogService;
        _model = new Pokemon();

        MoveItems = _idToNameService.GetComboBoxItemsExceptDefault<IMoveService>();
        AbilityItems = _idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();

        _evolutionEntryOptions = _idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        JumpToMoveCommand = new RelayCommand<int>(id => jumpService.JumpTo(MoveSelectorEditorModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));
        AddEvolutionCommand = new RelayCommand(AddEvolution);
        RemoveEvolutionCommand = new RelayCommand(RemoveEvolution, () => Evolutions.Count > 0);
        ViewSpritesCommand = new RelayCommand(ViewSprites);
        ImportAnimationCommand = new RelayCommand(ImportAnimation);
        ExportAnimationsCommand = new RelayCommand(ExportAnimations);
        RevertAnimationCommand = new RelayCommand(RevertAnimation, () => IsAnimationOverwritten);

        CopyPasteVm.ModelPasted += (_, __) => SetModel(_id, _model);
    }

    public void SetModel(PokemonId id, Pokemon model)
    {
        _model = model;
        _id = id;
        CopyPasteVm.Model = model;
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
        RaiseAllPropertiesChanged();
        RemoveEvolutionCommand.RaiseCanExecuteChanged();
    }

    public ICommand ImportAnimationCommand { get; }
    public RelayCommand RevertAnimationCommand { get; }
    public ICommand ExportAnimationsCommand { get; }
    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public List<SelectorComboBoxItem> MoveItems { get; }
    public List<SelectorComboBoxItem> AbilityItems { get; }

    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, v => _model.Name = v);
    }

    public TypeId Type1
    {
        get => _model.Type1;
        set => SetProperty(_model.Type1, value, v => _model.Type1 = v);
    }
    public TypeId Type2
    {
        get => _model.Type2;
        set => SetProperty(_model.Type2, value, v => _model.Type2 = v);
    }

    public int Move
    {
        get => (int)_model.Move;
        set => SetProperty(_model.Move, (MoveId)value, v => _model.Move = v);
    }

    public int Ability1
    {
        get => (int)_model.Ability1;
        set => SetProperty(_model.Ability1, (AbilityId)value, v => _model.Ability1 = v);
    }
    public int Ability2
    {
        get => (int)_model.Ability2;
        set => SetProperty(_model.Ability2, (AbilityId)value, v => _model.Ability2 = v);
    }
    public int Ability3
    {
        get => (int)_model.Ability3;
        set => SetProperty(_model.Ability3, (AbilityId)value, v => _model.Ability3 = v);
    }

    public int Hp
    {
        get => _model.Hp;
        set => SetProperty(_model.Hp, value, v => _model.Hp = v);
    }

    public int Atk
    {
        get => _model.Atk;
        set => SetProperty(_model.Atk, value, v => _model.Atk = v);
    }

    public int Def
    {
        get => _model.Def;
        set => SetProperty(_model.Def, value, v => _model.Def = v);
    }

    public int Spe
    {
        get => _model.Spe;
        set => SetProperty(_model.Spe, value, v => _model.Spe = v);
    }

    public bool IsLegendary
    {
        get => _model.IsLegendary;
        set => SetProperty(_model.IsLegendary, value, v => _model.IsLegendary = v);
    }

    public IdleMotionId IdleMotion
    {
        get => _model.IdleMotion;
        set => SetProperty(_model.IdleMotion, value, v => _model.IdleMotion = v);
    }

    public int NameOrderIndex
    {
        get => _model.NameOrderIndex;
        set => SetProperty(_model.NameOrderIndex, value, v => _model.NameOrderIndex = v);
    }

    public int NationalPokedexNumber
    {
        get => _model.NationalPokedexNumber;
        set => SetProperty(_model.NationalPokedexNumber, value, v => _model.NationalPokedexNumber = v);
    }

    public int MovementRange
    {
        get => _model.MovementRange;
        set => SetProperty(_model.MovementRange, value, v => _model.MovementRange = value);
    }

    public int CatchRate
    {
        get => _model.CatchRate;
        set => SetProperty(_model.CatchRate, value, v => _model.CatchRate = v);
    }

    public int UnknownAnimationValue
    {
        get => _model.UnknownAnimationValue;
        set => SetProperty(_model.UnknownAnimationValue, value, v => _model.UnknownAnimationValue = v);
    }

    public int BattleIntroSpriteOffset
    {
        get => _model.BattleIntroSpriteOffset;
        set => SetProperty(_model.BattleIntroSpriteOffset, value, v => _model.BattleIntroSpriteOffset = v);
    }

    public int Weight
    {
        get => _model.Weight;
        set => SetProperty(_model.Weight, value, v => _model.Weight = v);
    }

    public bool AsymmetricBattleSprite
    {
        get => _model.AsymmetricBattleSprite;
        set => SetProperty(_model.AsymmetricBattleSprite, value, v => _model.AsymmetricBattleSprite = v);
    }

    public bool LongAttackAnimation
    {
        get => _model.LongAttackAnimation;
        set => SetProperty(_model.LongAttackAnimation, value, v => _model.LongAttackAnimation = v);
    }

    public EvolutionConditionId EvolutionCondition1
    {
        get => _model.EvolutionCondition1;
        set
        {
            if (SetProperty(_model.EvolutionCondition1, value, v => _model.EvolutionCondition1 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition1Name));
            }
        }
    }

    public int QuantityForEvolutionCondition1
    {
        get => _model.QuantityForEvolutionCondition1;
        set 
        {
            if (SetProperty(_model.QuantityForEvolutionCondition1, value, v => _model.QuantityForEvolutionCondition1 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition1Name));
            }
        }
    }

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

    public EvolutionConditionId EvolutionCondition2
    {
        get => _model.EvolutionCondition2;
        set
        {
            if (SetProperty(_model.EvolutionCondition2, value, v => _model.EvolutionCondition2 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition2Name));
            }
        }
    }

    public int QuantityForEvolutionCondition2
    {
        get => _model.QuantityForEvolutionCondition2;
        set
        {
            if (SetProperty(_model.QuantityForEvolutionCondition2, value, v => _model.QuantityForEvolutionCondition2 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition2Name));
            }
        }
    }

    public string QuantityForEvolutionCondition2Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition2, _model.QuantityForEvolutionCondition2);

    public ObservableCollection<HabitatItem> HabitatItems { get; } = new();

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

    public CopyPasteViewModel CopyPasteVm { get; }
}
