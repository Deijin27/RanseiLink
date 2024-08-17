using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

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

    public int Effect1Amount
    {
        get => GetInt(3, 24, 2);
        set => SetInt(3, 24, 2, value);
    }

    public AbilityEffectId Effect1
    {
        get => (AbilityEffectId)GetInt(4, 0, 5);
        set => SetInt(4, 0, 5, (int)value);
    }

    public AbilityEffectId Effect2
    {
        get => (AbilityEffectId)GetInt(4, 5, 5);
        set => SetInt(4, 5, 5, (int)value);
    }

    public int Effect2Amount
    {
        get => GetInt(4, 10, 2);
        set => SetInt(4, 10, 2, value);
    }
}