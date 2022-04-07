using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class Building : BaseDataWindow
{
    public const int DataLength = 0x24;
    public Building(byte[] data) : base(data, DataLength) { }
    public Building() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 0x12);
        set => SetPaddedUtf8String(0, 0x12, value);
    }

    public KingdomId Kingdom
    {
        get => (KingdomId)GetByte(27);
        set => SetByte(27, (byte)value);
    }

    public BattleConfigId BattleConfig1
    {
        get => (BattleConfigId)GetUInt32(7, 7, 0);
        set => SetUInt32(7, 7, 0, (uint)value);
    }

    public BattleConfigId BattleConfig2
    {
        get => (BattleConfigId)GetUInt32(7, 7, 7);
        set => SetUInt32(7, 7, 7, (uint)value);
    }

    public BattleConfigId BattleConfig3
    {
        get => (BattleConfigId)GetUInt32(8, 7, 0);
        set => SetUInt32(8, 7, 0, (uint)value);
    }

    public BuildingSpriteId Sprite1
    {
        get => (BuildingSpriteId)GetUInt32(8, 7, 7);
        set => SetUInt32(8, 7, 7, (uint)value);
    }

    public BuildingSpriteId Sprite2
    {
        get => (BuildingSpriteId)GetUInt32(8, 7, 14);
        set => SetUInt32(8, 7, 14, (uint)value);
    }

    public BuildingSpriteId Sprite3
    {
        get => (BuildingSpriteId)GetUInt32(8, 7, 21);
        set => SetUInt32(8, 7, 21, (uint)value);
    }

    public BuildingFunctionId Function
    {
        get => (BuildingFunctionId)GetUInt32(8, 4, 28);
        set => SetUInt32(8, 4, 28, (uint)value);
    }

}
