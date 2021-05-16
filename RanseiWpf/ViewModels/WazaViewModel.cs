using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;
using Core.Enums;
using Core;
using System.Linq;
using Core.Models.Interfaces;

namespace RanseiWpf.ViewModels
{
    public class WazaViewModel : ViewModelBase, IViewModelForModel<IMove>
    {
        public IMove Model { get; set; }

        public string Name
        {
            get => Model.Name;
            set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
        }

        public bool MovementFlag_MovementOrKnockback
        {
            get => (Model.MovementFlags & MoveMovementFlags.MovementOrKnockback) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_MovementOrKnockback, value, v => Model.MovementFlags ^= MoveMovementFlags.MovementOrKnockback);
        }

        public bool MovementFlag_InvertMovementDirection
        {
            get => (Model.MovementFlags & MoveMovementFlags.InvertMovementDirection) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_InvertMovementDirection, value, v => Model.MovementFlags ^= MoveMovementFlags.InvertMovementDirection);
        }

        public bool MovementFlag_DoubleMovementDistance
        {
            get => (Model.MovementFlags & MoveMovementFlags.DoubleMovementDistance) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_DoubleMovementDistance, value, v => Model.MovementFlags ^= MoveMovementFlags.DoubleMovementDistance);
        }

        public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
        public TypeId Type
        {
            get => Model.Type;
            set => RaiseAndSetIfChanged(Model.Type, value, v => Model.Type = v);
        }

        public uint Power
        {
            get => Model.Power;
            set => RaiseAndSetIfChanged(Model.Power, value, v => Model.Power = v);
        }

        public uint Accuracy
        {
            get => Model.Accuracy;
            set => RaiseAndSetIfChanged(Model.Accuracy, value, v => Model.Accuracy = v);
        }

        public MoveRangeId[] RangeItems { get; } = EnumUtil.GetValues<MoveRangeId>().ToArray();
        public MoveRangeId Range
        {
            get => Model.Range;
            set => RaiseAndSetIfChanged(Model.Range, value, v => Model.Range = v);
        }

        public MoveEffectId[] EffectItems { get; } = EnumUtil.GetValues<MoveEffectId>().ToArray();

        public MoveEffectId Effect0
        {
            get => Model.Effect0;
            set => RaiseAndSetIfChanged(Model.Effect0, value, v => Model.Effect0 = v);
        }

        public uint Effect0Chance
        {
            get => Model.Effect0Chance;
            set => RaiseAndSetIfChanged(Model.Effect0Chance, value, v => Model.Effect0Chance = v);
        }

        public MoveEffectId Effect1
        {
            get => Model.Effect1;
            set => RaiseAndSetIfChanged(Model.Effect1, value, v => Model.Effect1 = v);
        }

        public uint Effect1Chance
        {
            get => Model.Effect1Chance;
            set => RaiseAndSetIfChanged(Model.Effect1Chance, value, v => Model.Effect1Chance = v);
        }

    }
}
