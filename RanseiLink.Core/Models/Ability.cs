using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

/// <summary>
/// Tokusei
/// </summary>
public class Ability : BaseDataWindow
{
    public const int DataLength = 0x14;
    public Ability(byte[] data) : base(data, DataLength) { }
    public Ability() : this(new byte[DataLength]) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 14);
        set => SetPaddedUtf8String(0, 14, value);
    }

    public uint Effect1Amount
    {
        get => GetUInt32(3, 24, 2);
        set => SetUInt32(3, 24, 2, value);
    }

    public AbilityEffectId Effect1
    {
        get => (AbilityEffectId)GetUInt32(4, 0, 5);
        set => SetUInt32(4, 0, 5, (uint)value);
    }

    public AbilityEffectId Effect2
    {
        get => (AbilityEffectId)GetUInt32(4, 5, 5);
        set => SetUInt32(4, 5, 5, (uint)value);
    }

    public uint Effect2Amount
    {
        get => GetUInt32(4, 10, 2);
        set => SetUInt32(4, 10, 2, value);
    }
}
