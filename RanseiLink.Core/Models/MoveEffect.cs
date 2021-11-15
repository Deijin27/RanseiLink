
namespace RanseiLink.Core.Models;

public class MoveEffect : BaseDataWindow
{
    public const int DataLength = 4;
    public MoveEffect(byte[] data) : base(data, DataLength) { }
    public MoveEffect() : base(new byte[DataLength], DataLength) { }

    public uint UnknownA
    {
        get => GetUInt16(0);
        set => SetUInt16(0, (ushort)value);
    }

    public uint UnknownB
    {
        get => GetUInt16(2);
        set => SetUInt16(2, (ushort)value);
    }
}
