using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class Kingdom : BaseDataWindow
{
    public const int DataLength = 0x18;
    public Kingdom(byte[] data) : base(data, DataLength) { }

    public Kingdom() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 10);
        set => SetPaddedUtf8String(0, 10, value);
    }

    public uint Unknown_R2_C24_L3
    {
        get => GetUInt32(2, 24, 3);
        set => SetUInt32(2, 24, 3, value);
    }

    #region Kingdoms you can battle using warriors in this kingdom

    public KingdomId MapConnection0
    {
        get => (KingdomId)GetUInt32(2, 27, 5);
        set => SetUInt32(2, 27, 5, (uint)value);
    }

    public KingdomId MapConnection1
    {
        get => (KingdomId)GetUInt32(3, 0, 5);
        set => SetUInt32(3, 0, 5, (uint)value);
    }

    public KingdomId MapConnection2
    {
        get => (KingdomId)GetUInt32(3, 5, 5);
        set => SetUInt32(3, 5, 5, (uint)value);
    }

    public KingdomId MapConnection3
    {
        get => (KingdomId)GetUInt32(3, 10, 5);
        set => SetUInt32(3, 10, 5, (uint)value);
    }

    public KingdomId MapConnection4
    {
        get => (KingdomId)GetUInt32(4, 0, 5);
        set => SetUInt32(4, 0, 5, (uint)value);
    }

    public KingdomId MapConnection5
    {
        get => (KingdomId)GetUInt32(4, 5, 5);
        set => SetUInt32(4, 5, 5, (uint)value);
    }

    public KingdomId MapConnection6
    {
        get => (KingdomId)GetUInt32(4, 10, 5);
        set => SetUInt32(4, 10, 5, (uint)value);
    }

    public KingdomId MapConnection7
    {
        get => (KingdomId)GetUInt32(4, 15, 5);
        set => SetUInt32(4, 15, 5, (uint)value);
    }

    public KingdomId MapConnection8
    {
        get => (KingdomId)GetUInt32(4, 20, 5);
        set => SetUInt32(4, 20, 5, (uint)value);
    }

    public KingdomId MapConnection9
    {
        get => (KingdomId)GetUInt32(4, 25, 5);
        set => SetUInt32(4, 25, 5, (uint)value);
    }

    public KingdomId MapConnection10
    {
        get => (KingdomId)GetUInt32(5, 0, 5);
        set => SetUInt32(5, 0, 5, (uint)value);
    }

    public KingdomId MapConnection11
    {
        get => (KingdomId)GetUInt32(5, 5, 5);
        set => SetUInt32(5, 5, 5, (uint)value);
    }

    public KingdomId MapConnection12
    {
        get => (KingdomId)GetUInt32(5, 10, 5);
        set => SetUInt32(5, 10, 5, (uint)value);
    }

    public KingdomId[] MapConnections
    {
        get => new[] { MapConnection0, MapConnection1, MapConnection2, MapConnection3, MapConnection4, MapConnection5, MapConnection6, MapConnection7, MapConnection8, MapConnection9, MapConnection10, MapConnection11, MapConnection12 };
        set
        {
            MapConnection0 = value[0];
            MapConnection1 = value[1];
            MapConnection2 = value[2];
            MapConnection3 = value[3];
            MapConnection4 = value[4];
            MapConnection5 = value[5];
            MapConnection6 = value[6];
            MapConnection7 = value[7];
            MapConnection8 = value[8];
            MapConnection9 = value[9];
            MapConnection10 = value[10];
            MapConnection11 = value[11];
            MapConnection12 = value[12];
        }
    }

    #endregion

    public BattleConfigId BattleConfig
    {
        get => (BattleConfigId)GetUInt32(5, 15, 7);
        set => SetUInt32(5, 15, 7, (uint)value);
    }

    public uint Unknown_R5_C22_L4
    {
        get => GetUInt32(5, 22, 4);
        set => SetUInt32(5, 22, 4, value);
    }


    public uint Unknown_R5_C26_L4
    {
        get => GetUInt32(5, 26, 4);
        set => SetUInt32(5, 26, 4, value);
    }
    
}
