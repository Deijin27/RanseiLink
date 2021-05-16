using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    /// <summary>
    /// Tokusei
    /// </summary>
    public class Ability : BaseDataWindow, IAbility
    {
        public const int DataLength = 0x14;
        public Ability(byte[] data) : base(data, DataLength) { }
        public Ability() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x0C);
            set => SetPaddedUtf8String(0, 0x0C, value);
        }

        public uint Effect1Amount
        {
            get => GetUInt32(3, 2, 24);
            set => SetUInt32(3, 2, 24, value);
        }

        public AbilityEffectId Effect1
        {
            get => (AbilityEffectId)GetUInt32(4, 5, 0);
            set => SetUInt32(4, 5, 0, (uint)value);
        }

        public AbilityEffectId Effect2
        {
            get => (AbilityEffectId)GetUInt32(4, 5, 5);
            set => SetUInt32(4, 5, 5, (uint)value);
        }

        public uint Effect2Amount
        {
            get => GetUInt32(4, 2, 10);
            set => SetUInt32(4, 2, 10, value);
        }

        public IAbility Clone()
        {
            return new Ability((byte[])Data.Clone());
        }
    }
}
