using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;
using System;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate PokemonViewModel PokemonViewModelFactory(IPokemon model, IEditorContext context);

public abstract class PokemonViewModelBase : ViewModelBase
{
    protected readonly IPokemon _model;
    public PokemonViewModelBase(IPokemon model)
    {
        _model = model;
        UpdateEvolution();
    }

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

    public MoveId Move
    {
        get => _model.Move;
        set => RaiseAndSetIfChanged(_model.Move, value, v => _model.Move = v);
    }

    public AbilityId Ability1
    {
        get => _model.Ability1;
        set => RaiseAndSetIfChanged(_model.Ability1, value, v => _model.Ability1 = v);
    }
    public AbilityId Ability2
    {
        get => _model.Ability2;
        set => RaiseAndSetIfChanged(_model.Ability2, value, v => _model.Ability2 = v);
    }
    public AbilityId Ability3
    {
        get => _model.Ability3;
        set => RaiseAndSetIfChanged(_model.Ability3, value, v => _model.Ability3 = v);
    }

    public uint Hp
    {
        get => _model.Hp;
        set => RaiseAndSetIfChanged(_model.Hp, value, v => _model.Hp = v);
    }

    public uint Atk
    {
        get => _model.Atk;
        set => RaiseAndSetIfChanged(_model.Atk, value, v => _model.Atk = v);
    }

    public uint Def
    {
        get => _model.Def;
        set => RaiseAndSetIfChanged(_model.Def, value, v => _model.Def = v);
    }

    public uint Spe
    {
        get => _model.Spe;
        set => RaiseAndSetIfChanged(_model.Spe, value, v => _model.Spe = v);
    }

    public bool IsLegendary
    {
        get => _model.IsLegendary;
        set => RaiseAndSetIfChanged(_model.IsLegendary, value, v => _model.IsLegendary = v);
    }

    public uint NameOrderIndex
    {
        get => _model.NameOrderIndex;
        set => RaiseAndSetIfChanged(_model.NameOrderIndex, value, v => _model.NameOrderIndex = v);
    }

    public uint NationalPokedexNumber
    {
        get => _model.NationalPokedexNumber;
        set => RaiseAndSetIfChanged(_model.NationalPokedexNumber, value, v => _model.NationalPokedexNumber = v);
    }

    public uint MovementRange
    {
        get => _model.MovementRange;
        set => RaiseAndSetIfChanged(_model.MovementRange, value, v => _model.MovementRange = value);
    }

    private uint _minEvolutionEntry;
    public uint MinEvolutionEntry
    {
        get => _minEvolutionEntry;
        set
        {
            _model.EvolutionRange = new PokemonEvolutionRange()
            {
                CanEvolve = _canEvolve,
                MaxEntry = _maxEvolutionEntry,
                MinEntry = value
            };
            UpdateEvolution();
        }
    }

    private uint _maxEvolutionEntry;
    public uint MaxEvolutionEntry
    {
        get => _maxEvolutionEntry;
        set
        {
            _model.EvolutionRange = new PokemonEvolutionRange()
            {
                CanEvolve = _canEvolve,
                MaxEntry = value,
                MinEntry = _minEvolutionEntry
            };
            UpdateEvolution();
        }
    }

    private bool _canEvolve;
    public bool CanEvolve
    {
        get => _canEvolve;
        set
        {
            _model.EvolutionRange = new PokemonEvolutionRange()
            {
                CanEvolve = value,
                MaxEntry = _maxEvolutionEntry,
                MinEntry = _minEvolutionEntry
            };
            UpdateEvolution();
        }
    }

    private void UpdateEvolution()
    {
        var newVal = _model.EvolutionRange;
        _canEvolve = newVal.CanEvolve;
        _minEvolutionEntry = newVal.MinEntry;
        _maxEvolutionEntry = newVal.MaxEntry;
        RaisePropertyChanged(nameof(CanEvolve));
        RaisePropertyChanged(nameof(MinEvolutionEntry));
        RaisePropertyChanged(nameof(MaxEvolutionEntry));
    }

    public uint UnknownValue
    {
        get => _model.UnknownValue;
        set => RaiseAndSetIfChanged(_model.UnknownValue, value, v => _model.UnknownValue = v);
    }
}

public class PokemonViewModel : PokemonViewModelBase
{
    public PokemonViewModel(IPokemon model, IEditorContext context) : base(model)
    {
        var jumpService = context.JumpService;

        JumpToMoveCommand = new RelayCommand<MoveId>(jumpService.JumpToMove);
        JumpToAbilityCommand = new RelayCommand<AbilityId>(jumpService.JumpToAbility);
    }

    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public EvolutionConditionId EvolutionCondition1
    {
        get => _model.EvolutionCondition1;
        set => RaiseAndSetIfChanged(_model.EvolutionCondition1, value, v => _model.EvolutionCondition1 = value);
    }

    public uint QuantityForEvolutionCondition1
    {
        get => _model.QuantityForEvolutionCondition1;
        set => RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition1, value, v => _model.QuantityForEvolutionCondition1 = value);
    }

    public EvolutionConditionId EvolutionCondition2
    {
        get => _model.EvolutionCondition2;
        set => RaiseAndSetIfChanged(_model.EvolutionCondition2, value, v => _model.EvolutionCondition2 = value);
    }
    public uint QuantityForEvolutionCondition2
    {
        get => _model.QuantityForEvolutionCondition2;
        set => RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition2, value, v => _model.QuantityForEvolutionCondition2 = value);
    }

    KingdomId _selectedEncounterKingdom;
    public KingdomId SelectedEncounterKingdom
    {
        get => _selectedEncounterKingdom;
        set
        {
            if (value != _selectedEncounterKingdom)
            {
                _selectedEncounterKingdom = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(EncounterableAtDefaultArea));
                RaisePropertyChanged(nameof(EncounterableWithLevel2Area));
            }

        }
    }

    public bool EncounterableAtDefaultArea
    {
        get => _model.GetEncounterable(SelectedEncounterKingdom, false);
        set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => _model.SetEncounterable(SelectedEncounterKingdom, false, v));
    }

    public bool EncounterableWithLevel2Area
    {
        get => _model.GetEncounterable(SelectedEncounterKingdom, true);
        set => RaiseAndSetIfChanged(EncounterableWithLevel2Area, value, v => _model.SetEncounterable(SelectedEncounterKingdom, true, v));
    }
}

public class PokemonGridItemViewModel : PokemonViewModelBase
{
    public PokemonGridItemViewModel(PokemonId id, IPokemon model) : base(model)
    {
        Id = id;
    }

    public PokemonId Id { get; }

    public new string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public EvolutionConditionId EvolutionCondition1
    {
        get => _model.EvolutionCondition1;
    }

    public string QuantityForEvolutionCondition1
    {
        get => FormatQuantity(EvolutionCondition1, _model.QuantityForEvolutionCondition1);
    }

    public EvolutionConditionId EvolutionCondition2
    {
        get => _model.EvolutionCondition2;
    }

    public string QuantityForEvolutionCondition2
    {
        get => FormatQuantity(EvolutionCondition2, _model.QuantityForEvolutionCondition2);
    }

    private static string FormatQuantity(EvolutionConditionId id, uint quantity)
    {
        switch (id)
        {
            case EvolutionConditionId.Hp:
            case EvolutionConditionId.Attack:
            case EvolutionConditionId.Defence:
            case EvolutionConditionId.Speed:
                return $"{quantity}";

            case EvolutionConditionId.Link:
                return $"{quantity}";

            case EvolutionConditionId.Kingdom:
                return $"{(KingdomId)quantity}";

            case EvolutionConditionId.WarriorGender:
                return $"{(GenderId)quantity}";

            case EvolutionConditionId.Item:
                return $"{(ItemId)quantity}";

            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                return "";

            default:
                throw new ArgumentException("Unexpected enum value");
        }
    }
}
