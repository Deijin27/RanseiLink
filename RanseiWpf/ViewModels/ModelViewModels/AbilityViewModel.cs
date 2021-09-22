using Core.Models;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using System.Linq;
using Core.Models.Interfaces;

namespace RanseiWpf.ViewModels
{
    public class AbilityViewModel : ViewModelBase, IViewModelForModel<IAbility>
    {
        public IAbility Model { get; set; }

        public string Name
        {
            get => Model.Name;
            set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
        }

        public AbilityEffectId[] EffectItems { get; } = EnumUtil.GetValues<AbilityEffectId>().ToArray();
        public AbilityEffectId Effect1
        {
            get => Model.Effect1;
            set => RaiseAndSetIfChanged(Model.Effect1, value, v => Model.Effect1 = v);
        }

        public uint Effect1Amount
        {
            get => Model.Effect1Amount;
            set => RaiseAndSetIfChanged(Model.Effect1Amount, value, v => Model.Effect1Amount = value);
        }

        public AbilityEffectId Effect2
        {
            get => Model.Effect2;
            set => RaiseAndSetIfChanged(Model.Effect2, value, v => Model.Effect2 = v);
        }

        public uint Effect2Amount
        {
            get => Model.Effect2Amount;
            set => RaiseAndSetIfChanged(Model.Effect2Amount, value, v => Model.Effect2Amount = value);
        }

    }
}
