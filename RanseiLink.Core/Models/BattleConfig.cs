using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Types;

namespace RanseiLink.Core.Models;

public class BattleConfig : BaseDataWindow, IBattleConfig
{
    public const int DataLength = 0x18;
    public BattleConfig(byte[] data) : base(data, DataLength) { }
    public BattleConfig() : this(new byte[DataLength]) { }

    public uint Environment
    {
        get => GetUInt32(0, 6, 0);
        set => SetUInt32(0, 6, 0, value);
    }

    public uint EnvironmentVariant
    {
        get => GetUInt32(0, 5, 6);
        set => SetUInt32(0, 5, 6, value);
    }

    public Rgb555 UpperAtmosphereColor
    {
        get => Rgb555.From(GetUInt32(0, 15, 11));
        set => SetUInt32(0, 15, 11, value.ToUInt32());
    }

    public Rgb555 MiddleAtmosphereColor
    {
        get => Rgb555.From(GetUInt32(1, 15, 0));
        set => SetUInt32(1, 15, 0, value.ToUInt32());
    }

    public Rgb555 LowerAtmosphereColor
    {
        get => Rgb555.From(GetUInt32(1, 15, 15));
        set => SetUInt32(1, 15, 15, value.ToUInt32());
    }

    public BattleVictoryConditionFlags VictoryCondition
    {
        get => (BattleVictoryConditionFlags)GetUInt32(2, 5, 0);
        set => SetUInt32(2, 5, 0, (uint)value);
    }

    public BattleVictoryConditionFlags DefeatCondition
    {
        get => (BattleVictoryConditionFlags)GetUInt32(2, 5, 5);
        set => SetUInt32(2, 5, 5, (uint)value);
    }

    public uint NumberOfTurns
    {
        get => GetUInt32(2, 5, 24);
        set => SetUInt32(2, 5, 24, value);
    }

    public IBattleConfig Clone()
    {
        return new BattleConfig((byte[])Data.Clone());
    }
}
