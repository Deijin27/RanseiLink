using Core.Structs;
using Core.Enums;

namespace Core.Models
{
    /// <summary>
    /// Tokusei
    /// </summary>
    public class Ability : BaseDataWindow
    {
        public const int DataLength = 0x14;
        public Ability(byte[] data) : base(data, DataLength) { }
        public Ability() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x0C);
            set => SetPaddedUtf8String(0, 0x0C, value);
        }

        public UInt2 Effect1Amount 
        { 
            get => (UInt2)(GetUInt32(3 * 4) >> 24); 
        }

        public AbilityEffectId Effect1
        {
            get => (AbilityEffectId)(int)(UInt5)GetUInt32(4 * 4);
        }

        public AbilityEffectId Effect2
        {
            get => (AbilityEffectId)(int)(UInt5)(GetUInt32(4 * 4) >> 5);
        }

        public UInt2 Effect2Amount
        {
            get => (UInt2)(GetUInt32(4 * 4) >> 10);
        }
    }
}
