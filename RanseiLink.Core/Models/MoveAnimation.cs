
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class MoveAnimation : BaseDataWindow, IMoveAnimation
{
    public const int DataLength = 4;
    public MoveAnimation(byte[] data) : base(data, DataLength) { }
    public MoveAnimation() : this(new byte[DataLength]) { }

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

    public IMoveAnimation Clone()
    {
        return new MoveAnimation((byte[])Data.Clone());
    }
}
