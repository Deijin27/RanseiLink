using Core;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class PokemonViewModel : ViewModelBase, IViewModelForModel<IPokemon>
    {
        public IPokemon Model { get; set; }

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

        public EvolutionConditionId[] EvolutionConditionItems { get; } = EnumUtil.GetValues<EvolutionConditionId>().ToArray();

        public EvolutionConditionId EvolutionCondition1
        {
            get => Model.EvolutionCondition1;
            set => RaiseAndSetIfChanged(Model.EvolutionCondition1, value, v => Model.EvolutionCondition1 = value);
        }

        public EvolutionConditionId EvolutionCondition2
        {
            get => Model.EvolutionCondition2;
            set => RaiseAndSetIfChanged(Model.EvolutionCondition2, value, v => Model.EvolutionCondition2 = value);
        }

        public LocationId[] LocationItems { get; } = EnumUtil.GetValues<LocationId>().ToArray();
        LocationId _selectedEncounterLocation;
        public LocationId SelectedEncounterLocation
        {
            get => _selectedEncounterLocation;
            set
            {
                if (value != _selectedEncounterLocation)
                {
                    _selectedEncounterLocation = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(EncounterableAtDefaultArea));
                    RaisePropertyChanged(nameof(EncounterableWithLevel2Area));
                }
                
            }
        }

        public bool EncounterableAtDefaultArea
        {
            get => Model.GetEncounterable(SelectedEncounterLocation, false);
            set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => Model.SetEncounterable(SelectedEncounterLocation, false, v));
        }

        public bool EncounterableWithLevel2Area
        {
            get => Model.GetEncounterable(SelectedEncounterLocation, true);
            set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => Model.SetEncounterable(SelectedEncounterLocation, true, v));
        }


    }
}
