using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public class PokemonViewModel : ViewModelBase, IViewModelForModel<IPokemon>
{
    public PokemonViewModel()
    {

    }

    private IPokemon _model;
    public IPokemon Model
    {
        get => _model;
        set
        {
            _model = value;
            UpdateEvolution();
        }
    }

    public string Name
    {
        get => Model.Name;
        set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
    }

    public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
    public TypeId Type1
    {
        get => Model.Type1;
        set => RaiseAndSetIfChanged(Model.Type1, value, v => Model.Type1 = v);
    }
    public TypeId Type2
    {
        get => Model.Type2;
        set => RaiseAndSetIfChanged(Model.Type2, value, v => Model.Type2 = v);
    }

    public MoveId[] MoveItems { get; } = EnumUtil.GetValues<MoveId>().ToArray();
    public MoveId Move
    {
        get => Model.Move;
        set => RaiseAndSetIfChanged(Model.Move, value, v => Model.Move = v);
    }

    public AbilityId[] AbilityItems { get; } = EnumUtil.GetValues<AbilityId>().ToArray();
    public AbilityId Ability1
    {
        get => Model.Ability1;
        set => RaiseAndSetIfChanged(Model.Ability1, value, v => Model.Ability1 = v);
    }
    public AbilityId Ability2
    {
        get => Model.Ability2;
        set => RaiseAndSetIfChanged(Model.Ability2, value, v => Model.Ability2 = v);
    }
    public AbilityId Ability3
    {
        get => Model.Ability3;
        set => RaiseAndSetIfChanged(Model.Ability3, value, v => Model.Ability3 = v);
    }

    public uint Hp
    {
        get => Model.Hp;
        set => RaiseAndSetIfChanged(Model.Hp, value, v => Model.Hp = v);
    }

    public uint Atk
    {
        get => Model.Atk;
        set => RaiseAndSetIfChanged(Model.Atk, value, v => Model.Atk = v);
    }

    public uint Def
    {
        get => Model.Def;
        set => RaiseAndSetIfChanged(Model.Def, value, v => Model.Def = v);
    }

    public uint Spe
    {
        get => Model.Spe;
        set => RaiseAndSetIfChanged(Model.Spe, value, v => Model.Spe = v);
    }

    public bool IsLegendary
    {
        get => Model.IsLegendary;
        set => RaiseAndSetIfChanged(Model.IsLegendary, value, v => Model.IsLegendary = v);
    }

    public uint NameOrderIndex
    {
        get => Model.NameOrderIndex;
        set => RaiseAndSetIfChanged(Model.NameOrderIndex, value, v => Model.NameOrderIndex = v);
    }

    public uint NationalPokedexNumber
    {
        get => Model.NationalPokedexNumber;
        set => RaiseAndSetIfChanged(Model.NationalPokedexNumber, value, v => Model.NationalPokedexNumber = v);
    }

    public EvolutionConditionId[] EvolutionConditionItems { get; } = EnumUtil.GetValues<EvolutionConditionId>().ToArray();

    public EvolutionConditionId EvolutionCondition1
    {
        get => Model.EvolutionCondition1;
        set => RaiseAndSetIfChanged(Model.EvolutionCondition1, value, v => Model.EvolutionCondition1 = value);
    }

    public uint QuantityForEvolutionCondition1
    {
        get => Model.QuantityForEvolutionCondition1;
        set => RaiseAndSetIfChanged(Model.QuantityForEvolutionCondition1, value, v => Model.QuantityForEvolutionCondition1 = value);
    }

    public EvolutionConditionId EvolutionCondition2
    {
        get => Model.EvolutionCondition2;
        set => RaiseAndSetIfChanged(Model.EvolutionCondition2, value, v => Model.EvolutionCondition2 = value);
    }
    public uint QuantityForEvolutionCondition2
    {
        get => Model.QuantityForEvolutionCondition2;
        set => RaiseAndSetIfChanged(Model.QuantityForEvolutionCondition2, value, v => Model.QuantityForEvolutionCondition2 = value);
    }

    public uint MovementRange
    {
        get => Model.MovementRange;
        set => RaiseAndSetIfChanged(Model.MovementRange, value, v => Model.MovementRange = value);
    }

    private uint _minEvolutionEntry;
    public uint MinEvolutionEntry
    {
        get => _minEvolutionEntry;
        set
        {
            Model.EvolutionRange = new PokemonEvolutionRange()
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
            Model.EvolutionRange = new PokemonEvolutionRange()
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
            Model.EvolutionRange = new PokemonEvolutionRange()
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
        var newVal = Model.EvolutionRange;
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
        get => Model.UnknownValue;
        set => RaiseAndSetIfChanged(Model.UnknownValue, value, v => Model.UnknownValue = v);
    }

    public bool EncounterableAtDefaultArea
    {
        get => Model.GetEncounterable(SelectedEncounterKingdom, false);
        set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => Model.SetEncounterable(SelectedEncounterKingdom, false, v));
    }

    public bool EncounterableWithLevel2Area
    {
        get => Model.GetEncounterable(SelectedEncounterKingdom, true);
        set => RaiseAndSetIfChanged(EncounterableWithLevel2Area, value, v => Model.SetEncounterable(SelectedEncounterKingdom, true, v));
    }


}
