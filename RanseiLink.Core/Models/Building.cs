using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class Building : BaseDataWindow, IBuilding
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

    public BattleMapId BattleMap1
    {
        get => (BattleMapId)GetUInt32(7, 7, 0);
        set => SetUInt32(7, 7, 0, (uint)value);
    }

    public BattleMapId BattleMap2
    {
        get => (BattleMapId)GetUInt32(7, 7, 7);
        set => SetUInt32(7, 7, 7, (uint)value);
    }

    public BattleMapId BattleMap3
    {
        get => (BattleMapId)GetUInt32(8, 7, 0);
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

    public IBuilding Clone()
    {
        return new Building((byte[])Data.Clone());
    }
}
