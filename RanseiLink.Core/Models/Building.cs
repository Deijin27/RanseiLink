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

    public IBuilding Clone()
    {
        return new Building((byte[])Data.Clone());
    }
}
