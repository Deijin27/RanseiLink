using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Graphics;

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

    public MapId MapId
    {
        get => new(GetUInt32(0, 6, 0), GetUInt32(0, 5, 6));
        set
        {
            SetUInt32(0, 6, 0, value.Map);
            SetUInt32(0, 5, 6, value.Variant);
        }
    }

    public Rgb15 UpperAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(0, 15, 11));
        set => SetUInt32(0, 15, 11, value.ToUInt16());
    }

    public Rgb15 MiddleAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(1, 15, 0));
        set => SetUInt32(1, 15, 0, value.ToUInt16());
    }

    public Rgb15 LowerAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(1, 15, 15));
        set => SetUInt32(1, 15, 15, value.ToUInt16());
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
