using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IPokemonViewModel 
{
    void SetModel(Pokemon model);
}

public class PokemonViewModel : ViewModelBase, IPokemonViewModel
{
    private Pokemon _model;
    private readonly List<SelectorComboBoxItem> _evolutionEntryOptions;
    private readonly IIdToNameService _idToNameService;
    private readonly IKingdomService _kingdomService;
    private readonly IItemService _itemService;
    public PokemonViewModel(IJumpService jumpService, IIdToNameService idToNameService, IKingdomService kingdomService, IItemService itemService)
    {
        _idToNameService = idToNameService;
        _kingdomService = kingdomService;
        _itemService = itemService;
        _model = new Pokemon();

        MoveItems = _idToNameService.GetComboBoxItemsExceptDefault<IMoveService>();
        AbilityItems = _idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();

        _evolutionEntryOptions = _idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        JumpToMoveCommand = new RelayCommand<int>(id => jumpService.JumpTo(MoveSelectorEditorModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));
        AddEvolutionCommand = new RelayCommand(AddEvolution);
        RemoveEvolutionCommand = new RelayCommand(RemoveEvolution);
    }

    public void SetModel(Pokemon model)
    {
        _model = model;
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
    }

    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public List<SelectorComboBoxItem> MoveItems { get; }
    public List<SelectorComboBoxItem> AbilityItems { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public TypeId Type1
    {
        get => _model.Type1;
        set => RaiseAndSetIfChanged(_model.Type1, value, v => _model.Type1 = v);
    }
    public TypeId Type2
    {
        get => _model.Type2;
        set => RaiseAndSetIfChanged(_model.Type2, value, v => _model.Type2 = v);
    }

    public int Move
    {
        get => (int)_model.Move;
        set => RaiseAndSetIfChanged(_model.Move, (MoveId)value, v => _model.Move = v);
    }

    public int Ability1
    {
        get => (int)_model.Ability1;
        set => RaiseAndSetIfChanged(_model.Ability1, (AbilityId)value, v => _model.Ability1 = v);
    }
    public int Ability2
    {
        get => (int)_model.Ability2;
        set => RaiseAndSetIfChanged(_model.Ability2, (AbilityId)value, v => _model.Ability2 = v);
    }
    public int Ability3
    {
        get => (int)_model.Ability3;
        set => RaiseAndSetIfChanged(_model.Ability3, (AbilityId)value, v => _model.Ability3 = v);
    }

    public int Hp
    {
        get => _model.Hp;
        set => RaiseAndSetIfChanged(_model.Hp, value, v => _model.Hp = v);
    }

    public int Atk
    {
        get => _model.Atk;
        set => RaiseAndSetIfChanged(_model.Atk, value, v => _model.Atk = v);
    }

    public int Def
    {
        get => _model.Def;
        set => RaiseAndSetIfChanged(_model.Def, value, v => _model.Def = v);
    }

    public int Spe
    {
        get => _model.Spe;
        set => RaiseAndSetIfChanged(_model.Spe, value, v => _model.Spe = v);
    }

    public bool IsLegendary
    {
        get => _model.IsLegendary;
        set => RaiseAndSetIfChanged(_model.IsLegendary, value, v => _model.IsLegendary = v);
    }

    public IdleMotionId IdleMotion
    {
        get => _model.IdleMotion;
        set => RaiseAndSetIfChanged(_model.IdleMotion, value, v => _model.IdleMotion = v);
    }

    public int NameOrderIndex
    {
        get => _model.NameOrderIndex;
        set => RaiseAndSetIfChanged(_model.NameOrderIndex, value, v => _model.NameOrderIndex = v);
    }

    public int NationalPokedexNumber
    {
        get => _model.NationalPokedexNumber;
        set => RaiseAndSetIfChanged(_model.NationalPokedexNumber, value, v => _model.NationalPokedexNumber = v);
    }

    public int MovementRange
    {
        get => _model.MovementRange;
        set => RaiseAndSetIfChanged(_model.MovementRange, value, v => _model.MovementRange = value);
    }

    public int CatchRate
    {
        get => _model.CatchRate;
        set => RaiseAndSetIfChanged(_model.CatchRate, value, v => _model.CatchRate = v);
    }

    public int UnknownAnimationValue
    {
        get => _model.UnknownAnimationValue;
        set => RaiseAndSetIfChanged(_model.UnknownAnimationValue, value, v => _model.UnknownAnimationValue = v);
    }

    public int UnknownValue2
    {
        get => _model.UnknownValue2;
        set => RaiseAndSetIfChanged(_model.UnknownValue2, value, v => _model.UnknownValue2 = v);
    }

    public bool AsymmetricBattleSprite
    {
        get => _model.AsymmetricBattleSprite;
        set => RaiseAndSetIfChanged(_model.AsymmetricBattleSprite, value, v => _model.AsymmetricBattleSprite = v);
    }

    public bool LongAttackAnimation
    {
        get => _model.LongAttackAnimation;
        set => RaiseAndSetIfChanged(_model.LongAttackAnimation, value, v => _model.LongAttackAnimation = v);
    }

    public EvolutionConditionId EvolutionCondition1
    {
        get => _model.EvolutionCondition1;
        set
        {
            if (RaiseAndSetIfChanged(_model.EvolutionCondition1, value, v => _model.EvolutionCondition1 = value))
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
            if (RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition1, value, v => _model.QuantityForEvolutionCondition1 = value))
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
            if (RaiseAndSetIfChanged(_model.EvolutionCondition2, value, v => _model.EvolutionCondition2 = value))
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
            if (RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition2, value, v => _model.QuantityForEvolutionCondition2 = value))
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
            set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => _model.SetEncounterable(_kingdom, false, v));
        }

        public bool EncounterableWithLevel2Area
        {
            get => _model.GetEncounterable(_kingdom, true);
            set => RaiseAndSetIfChanged(EncounterableWithLevel2Area, value, v => _model.SetEncounterable(_kingdom, true, v));
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
            set => RaiseAndSetIfChanged(Id, value, v => _evolutionTable[_id] = (PokemonId)v);
        }
        public List<SelectorComboBoxItem> Options { get; }
    }

    public ObservableCollection<EvolutionComboBoxItem> Evolutions { get; } = new();

    public ICommand AddEvolutionCommand { get; }
    public ICommand RemoveEvolutionCommand { get; }

    private void AddEvolution()
    {
        _model.Evolutions.Add(PokemonId.Eevee);
        var newItem = new EvolutionComboBoxItem(_model.Evolutions, _model.Evolutions.Count - 1, _evolutionEntryOptions);
        Evolutions.Add(newItem);
    }

    private void RemoveEvolution()
    {
        _model.Evolutions.RemoveAt(_model.Evolutions.Count - 1);
        Evolutions.RemoveAt(Evolutions.Count - 1);
    }
}

