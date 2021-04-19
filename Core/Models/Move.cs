using Core.Enums;
using Core.Structs;
using System.Text;

namespace Core.Models
{
    /// <summary>
    /// Waza
    /// </summary>
    public class Move : BaseDataWindow
    {
        public const int DataLength = 0x24;
        public Move(byte[] data) : base(data, DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 15);
            set => SetPaddedUtf8String(0, 15, value);
        }

        public MoveMovementFlags MovementFlags
        {
            get => (MoveMovementFlags)GetByte(0x0f);
        }

        public TypeId Type
        {
            get => (TypeId)(int)(UInt5)GetByte(0x10);
        }

        public UInt7 Power
        {
            get => (UInt7)(GetUInt16(0x10) >> 5);
        }

        public MoveEffectId Effect0
        {
            get => (MoveEffectId)(int)(UInt7)(GetUInt16(0x11) >> 5);
        }

        public UInt7 Effect0Chance
        {
            get => (UInt7)(GetUInt16(0x12) >> 4);
        }

        public MoveRangeId Range
        {
            get => (MoveRangeId)(int)(UInt5)(GetUInt16(0x13) >> 3);
        }

        public byte Something
        {
            get => GetByte(0x14);
        }

        public MoveEffectId Effect1
        {
            get => (MoveEffectId)(int)(UInt7)GetByte(24);
        }

        public UInt7 Effect1Chance
        {
            get => (UInt7)(GetUInt16(24) >> 7);
        }

        public MoveEffectId Effect2
        {
            get => (MoveEffectId)(int)(UInt7)(GetInt16(25) >> 6);
        }

        /// <summary>
        /// Percentage chance of secondary effect
        /// Also seems to carry Fixed Damage Amount (for dragon rage, the value is also stored in power?) 
        /// And for absorb and mega-drain it's 75 for some reason
        /// </summary>
        public UInt7 Effect2Chance
        {
            get => (UInt7)(GetUInt16(26) >> 5);
        }

        public MoveEffectId Effect3
        {
            get => (MoveEffectId)(int)(UInt7)GetByte(28);
        }

        public UInt7 Effect3Chance
        {
            get => (UInt7)(GetUInt16(28) >> 7);
        }

        public UInt5 Something1
        {
            get => (UInt5)(GetUInt16(29) >> 6);
        }

        public UInt7 Accuracy
        {
            get => (UInt7)(GetUInt16(30) >> 3);
        }

        public UInt6 ThingAfterAcc
        {
            get => (UInt6)(GetByte(31) >> 2);
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
