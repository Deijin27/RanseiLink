using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class Saihai : BaseDataWindow, ISaihai
    {
        public const int DataLength = 0x1C;
        public Saihai(byte[] data) : base(data, DataLength) { }
        public Saihai() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 15);
            set => SetPaddedUtf8String(0, 15, value);
        }

        public uint Effect1Amount
        {
            get => GetUInt32(4, 8, 24);
            set => SetUInt32(4, 8, 24, value);
        }

        public SaihaiEffectId Effect1
        {
            get => (SaihaiEffectId)GetUInt32(5, 7, 0);
            set => SetUInt32(5, 7, 0, (uint)value);
        }

        public SaihaiEffectId Effect2
        {
            get => (SaihaiEffectId)GetUInt32(5, 7, 7);
            set => SetUInt32(5, 7, 7, (uint)value);
        }

        public uint Effect2Amount
        {
            get => GetUInt32(5, 8, 14);
            set => SetUInt32(5, 8, 14, value);
        }

        public SaihaiEffectId Effect3
        {
            get => (SaihaiEffectId)GetUInt32(5, 7, 22);
            set => SetUInt32(5, 7, 22, (uint)value);
        }

        public uint Duration
        {
            get => GetUInt32(5, 3, 29);
            set => SetUInt32(5, 3, 29, value);
        }

        public uint Effect3Amount
        {
            get => GetUInt32(6, 8, 0);
            set => SetUInt32(6, 8, 0, value);
        }

        public SaihaiTargetId Target
        {
            get => (SaihaiTargetId)GetUInt32(6, 2, 8);
            set => SetUInt32(6, 2, 8, (uint)value);
        }

        public ISaihai Clone()
        {
            return new Saihai((byte[])Data.Clone());
        }
    }
}
