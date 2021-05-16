using Core.Enums;
using System.Text;
using Core.Models.Interfaces;

namespace Core.Models
{
    /// <summary>
    /// Waza
    /// </summary>
    public class Move : BaseDataWindow, IMove
    {
        public const int DataLength = 0x24;
        public Move(byte[] data) : base(data, DataLength) { }
        public Move() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 15);
            set => SetPaddedUtf8String(0, 15, value);
        }

        public MoveMovementFlags MovementFlags
        {
            get => (MoveMovementFlags)GetUInt32(3, 8, 24);
            set => SetUInt32(3, 8, 24, (uint)value);
        }

        public TypeId Type
        {
            get => (TypeId)GetUInt32(4, 5, 0);
            set => SetUInt32(4, 5, 0, (uint)value);
        }

        public uint Power
        {
            get => GetUInt32(4, 7, 5);
            set => SetUInt32(4, 7, 5, value);
        }

        public MoveEffectId Effect0
        {
            get => (MoveEffectId)GetUInt32(4, 7, 13);
            set => SetUInt32(4, 7, 13, (uint)value);
        }

        public uint Effect0Chance
        {
            get => GetUInt32(4, 7, 20);
            set => SetUInt32(4, 7, 20, value);
        }

        public MoveRangeId Range
        {
            get => (MoveRangeId)GetUInt32(4, 5, 27);
            set => SetUInt32(4, 5, 27, (uint)value);
        }

        public MoveEffectId Effect1
        {
            get => (MoveEffectId)GetUInt32(6, 7, 0);
            set => SetUInt32(6, 7, 0, (uint)value);
        }

        public uint Effect1Chance
        {
            get => GetUInt32(6, 7, 7);
            set => SetUInt32(6, 7, 7, value);
        }

        public MoveEffectId Effect2
        {
            get => (MoveEffectId)GetUInt32(6, 7, 14);
            set => SetUInt32(6, 7, 14, (uint)value);
        }

        public uint Effect2Chance
        {
            get => GetUInt32(6, 7, 21);
            set => SetUInt32(6, 7, 21, value);
        }

        public MoveEffectId Effect3
        {
            get => (MoveEffectId)GetUInt32(7, 7, 0);
            set => SetUInt32(7, 7, 0, (uint)value);
        }

        public uint Effect3Chance
        {
            get => GetUInt32(7, 7, 7);
            set => SetUInt32(7, 7, 7, value);
        }

        public uint Accuracy
        {
            get => GetUInt32(7, 7, 19);
            set => SetUInt32(7, 7, 19, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            void add(string tag, string content)
            {
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(": ");
                sb.AppendLine(content);
            }

            sb.AppendLine(nameof(Move) + ":");

            add(nameof(Name), Name);
            add(nameof(MovementFlags), MovementFlags.ToString());
            add(nameof(Type), Type.ToString());
            add(nameof(Power), Power.ToString());
            add(nameof(Effect0), Effect0.ToString());
            add(nameof(Effect0Chance), Effect0Chance.ToString());
            add(nameof(Range), Range.ToString());
            add(nameof(Effect1), Effect1.ToString());
            add(nameof(Effect1Chance), Effect1Chance.ToString());
            add(nameof(Effect2), Effect2.ToString());
            add(nameof(Effect2Chance), Effect2Chance.ToString());
            add(nameof(Effect3), Effect3.ToString());
            add(nameof(Effect3Chance), Effect3Chance.ToString());
            add(nameof(Accuracy), Accuracy.ToString());

            return sb.ToString();
        }
    }
}
