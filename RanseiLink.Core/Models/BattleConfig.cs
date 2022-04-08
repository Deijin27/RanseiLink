using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models;

public class BattleConfig : BaseDataWindow
{
    public const int DataLength = 0x18;
    public BattleConfig(byte[] data) : base(data, DataLength) { }
    public BattleConfig() : this(new byte[DataLength]) { }

    public uint Environment
    {
        get => GetUInt32(0, 0, 6);
        set => SetUInt32(0, 0, 6, value);
    }

    public uint EnvironmentVariant
    {
        get => GetUInt32(0, 6, 5);
        set => SetUInt32(0, 6, 5, value);
    }

    public MapId MapId
    {
        get => new((int)GetUInt32(0, 0, 6), (int)GetUInt32(0, 6, 5));
        set
        {
            SetUInt32(0, 0, 6, (uint)value.Map);
            SetUInt32(0, 6, 5, (uint)value.Variant);
        }
    }

    public Rgb15 UpperAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(0, 11, 15));
        set => SetUInt32(0, 11, 15, value.ToUInt16());
    }

    public Rgb15 MiddleAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(1, 0, 15));
        set => SetUInt32(1, 0, 15, value.ToUInt16());
    }

    public Rgb15 LowerAtmosphereColor
    {
        get => Rgb15.From((ushort)GetUInt32(1, 15, 15));
        set => SetUInt32(1, 15, 15, value.ToUInt16());
    }

    public BattleVictoryConditionFlags VictoryCondition
    {
        get => (BattleVictoryConditionFlags)GetUInt32(2, 0, 5);
        set => SetUInt32(2, 0, 5, (uint)value);
    }

    public BattleVictoryConditionFlags DefeatCondition
    {
        get => (BattleVictoryConditionFlags)GetUInt32(2, 5, 5);
        set => SetUInt32(2, 5, 5, (uint)value);
    }

    public uint NumberOfTurns
    {
        get => GetUInt32(2, 24, 5);
        set => SetUInt32(2, 24, 5, value);
    }

}
