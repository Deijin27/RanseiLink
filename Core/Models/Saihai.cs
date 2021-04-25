
using Core.Enums;
using Core.Structs;

namespace Core.Models
{
    public class Saihai : BaseDataWindow
    {
        public const int DataLength = 0x1C;
        public Saihai(byte[] data) : base(data, DataLength) { }
        public Saihai() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 15);
            set => SetPaddedUtf8String(0, 15, value);
        }

        public byte Effect1Amount
        {
            get => (byte)(GetUInt32(4 * 4) >> 24);
        }

        public SaihaiEffectId Effect1
        {
            get => (SaihaiEffectId)(int)(UInt7)GetUInt32(5 * 4);
        }

        public SaihaiEffectId Effect2
        {
            get => (SaihaiEffectId)(int)(UInt7)(GetUInt32(5 * 4) >> 7);
        }

        public byte Effect2Amount
        {
            get => (byte)(GetUInt32(5 * 4) >> 14);
        }

        public SaihaiEffectId Effect3
        {
            get => (SaihaiEffectId)(int)(UInt7)(GetUInt32(5 * 4) >> 22);
        }

        public UInt3 Duration
        {
            get => (UInt3)(GetUInt32(5 * 4) >> 29);
        }

        public byte Effect3Amount
        {
            get => (byte)(GetUInt32(6 * 4) >> 0);
        }

        public SaihaiTargetId Target
        {
            get => (SaihaiTargetId)(int)(UInt2)(GetUInt32(6 * 4) >> 8);
        }
    }
}
