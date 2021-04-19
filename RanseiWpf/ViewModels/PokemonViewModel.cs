using Core;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class PokemonViewModel : ViewModelBase, IViewModelForModel<Pokemon>
    {
        public Pokemon Model { get; set; }

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
    }
}
