using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class Gimmick : BaseDataWindow, IGimmick
{
    public const int DataLength = 0x28;
    public Gimmick(byte[] data) : base(data, DataLength) { }
    public Gimmick() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 16);
        set => SetPaddedUtf8String(0, 16, value);
    }

    /// <summary>
    /// If attack damage caused by this, this is the type of the attack
    /// </summary>
    public TypeId AttackType
    {
        get => (TypeId)GetUInt32(5, 5, 0);
        set => SetUInt32(5, 5, 0, (uint)value);
    }

    /// <summary>
    /// The type of attack that can destroy this gimmick
    /// </summary>
    public TypeId DestroyType
    {
        get => (TypeId)GetUInt32(5, 5, 5);
        set => SetUInt32(5, 5, 5, (uint)value);
    }

    /// <summary>
    /// Seems like a multipurpose quantity. For some, this is probably attack power, others something else
    /// </summary>
    public uint UnknownQuantity1
    {
        get => GetUInt32(6, 8, 0);
        set => SetUInt32(6, 8, 0, value);
    }

    public MoveAnimationId Animation1
    {
        get => (MoveAnimationId)GetUInt32(6, 8, 8);
        set => SetUInt32(6, 8, 8, (uint)value);
    }

    public MoveAnimationId Animation2
    {
        get => (MoveAnimationId)GetUInt32(6, 8, 16);
        set => SetUInt32(6, 8, 16, (uint)value);
    }

    public GimmickRangeId Range
    {
        get => (GimmickRangeId)GetUInt32(8, 5, 19);
        set => SetUInt32(8, 5, 19, (uint)value);
    }

    public IGimmick Clone()
    {
        return new Gimmick((byte[])Data.Clone());
    }
}
