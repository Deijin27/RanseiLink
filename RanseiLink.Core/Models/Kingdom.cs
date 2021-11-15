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

    public IKingdom Clone()
    {
        return new Kingdom((byte[])Data.Clone());
    }
}
