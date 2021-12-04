using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate PokemonViewModel PokemonViewModelFactory(IPokemon model);

public class PokemonViewModel : ViewModelBase
{
    private readonly IPokemon _model;
    public PokemonViewModel(IPokemon model)
    {
        _model = model;
        UpdateEvolution();
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
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

    public MoveId[] MoveItems { get; } = EnumUtil.GetValues<MoveId>().ToArray();
    public MoveId Move
    {
        get => _model.Move;
        set => RaiseAndSetIfChanged(_model.Move, value, v => _model.Move = v);
    }

    public AbilityId[] AbilityItems { get; } = EnumUtil.GetValues<AbilityId>().ToArray();
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

    public EvolutionConditionId[] EvolutionConditionItems { get; } = EnumUtil.GetValues<EvolutionConditionId>().ToArray();

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

    public KingdomId[] KingdomItems { get; } = EnumUtil.GetValues<KingdomId>().ToArray();
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

    public uint UnknownValue
    {
        get => _model.UnknownValue;
        set => RaiseAndSetIfChanged(_model.UnknownValue, value, v => _model.UnknownValue = v);
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
