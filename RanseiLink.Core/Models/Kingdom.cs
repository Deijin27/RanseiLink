using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class Kingdom : BaseDataWindow, IKingdom
{
    public const int DataLength = 0x18;
    public Kingdom(byte[] data) : base(data, DataLength) { }

    public Kingdom() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 10);
        set => SetPaddedUtf8String(0, 10, value);
    }

    #region Kingdoms you can battle using warriors in this kingdom

    public KingdomId MapConnection0
    {
        get => (KingdomId)GetUInt32(2, 5, 27);
        set => SetUInt32(2, 5, 27, (uint)value);
    }

    public KingdomId MapConnection1
    {
        get => (KingdomId)GetUInt32(3, 5, 0);
        set => SetUInt32(3, 5, 0, (uint)value);
    }

    public KingdomId MapConnection2
    {
        get => (KingdomId)GetUInt32(3, 5, 5);
        set => SetUInt32(3, 5, 5, (uint)value);
    }

    public KingdomId MapConnection3
    {
        get => (KingdomId)GetUInt32(3, 5, 10);
        set => SetUInt32(3, 5, 10, (uint)value);
    }

    public KingdomId MapConnection4
    {
        get => (KingdomId)GetUInt32(4, 5, 0);
        set => SetUInt32(4, 5, 0, (uint)value);
    }

    public KingdomId MapConnection5
    {
        get => (KingdomId)GetUInt32(4, 5, 5);
        set => SetUInt32(4, 5, 5, (uint)value);
    }

    public KingdomId MapConnection6
    {
        get => (KingdomId)GetUInt32(4, 5, 10);
        set => SetUInt32(4, 5, 10, (uint)value);
    }

    public KingdomId MapConnection7
    {
        get => (KingdomId)GetUInt32(4, 5, 15);
        set => SetUInt32(4, 5, 15, (uint)value);
    }

    public KingdomId MapConnection8
    {
        get => (KingdomId)GetUInt32(4, 5, 20);
        set => SetUInt32(4, 5, 20, (uint)value);
    }

    public KingdomId MapConnection9
    {
        get => (KingdomId)GetUInt32(4, 5, 25);
        set => SetUInt32(4, 5, 25, (uint)value);
    }

    public KingdomId MapConnection10
    {
        get => (KingdomId)GetUInt32(5, 5, 0);
        set => SetUInt32(5, 5, 0, (uint)value);
    }

    public KingdomId MapConnection11
    {
        get => (KingdomId)GetUInt32(5, 5, 5);
        set => SetUInt32(5, 5, 5, (uint)value);
    }

    public KingdomId MapConnection12
    {
        get => (KingdomId)GetUInt32(5, 5, 10);
        set => SetUInt32(5, 5, 10, (uint)value);
    }

    #endregion

    public BattleMapId BattleMap
    {
        get => (BattleMapId)GetUInt32(5, 7, 15);
        set => SetUInt32(5, 7, 15, (uint)value);
    }

    public IKingdom Clone()
    {
        return new Kingdom((byte[])Data.Clone());
    }
}
