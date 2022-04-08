using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

/// <summary>
/// Move
/// </summary>
public class Move : BaseDataWindow
{
    public const int DataLength = 0x24;
    public Move(byte[] data) : base(data, DataLength) { }
    public Move() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 14);
        set => SetPaddedUtf8String(0, 14, value);
    }

    public MoveMovementFlags MovementFlags
    {
        get => (MoveMovementFlags)GetUInt32(3, 24, 8);
        set => SetUInt32(3, 24, 8, (uint)value);
    }

    public TypeId Type
    {
        get => (TypeId)GetUInt32(4, 0, 5);
        set => SetUInt32(4, 0, 5, (uint)value);
    }

    public uint Power
    {
        get => GetUInt32(4, 5, 8);
        set => SetUInt32(4, 5, 8, value);
    }

    public MoveEffectId Effect1
    {
        get => (MoveEffectId)GetUInt32(4, 13, 7);
        set => SetUInt32(4, 13, 7, (uint)value);
    }

    public uint Effect1Chance
    {
        get => GetUInt32(4, 20, 7);
        set => SetUInt32(4, 20, 7, value);
    }

    public MoveRangeId Range
    {
        get => (MoveRangeId)GetUInt32(4, 27, 5);
        set => SetUInt32(4, 27, 5, (uint)value);
    }

    public MoveEffectId Effect2
    {
        get => (MoveEffectId)GetUInt32(6, 0, 7);
        set => SetUInt32(6, 0, 7, (uint)value);
    }

    public uint Effect2Chance
    {
        get => GetUInt32(6, 7, 7);
        set => SetUInt32(6, 7, 7, value);
    }

    public MoveEffectId Effect3
    {
        get => (MoveEffectId)GetUInt32(6, 14, 7);
        set => SetUInt32(6, 14, 7, (uint)value);
    }

    public uint Effect3Chance
    {
        get => GetUInt32(6, 21, 7);
        set => SetUInt32(6, 21, 7, value);
    }

    public MoveEffectId Effect4
    {
        get => (MoveEffectId)GetUInt32(7, 0, 7);
        set => SetUInt32(7, 0, 7, (uint)value);
    }

    public uint Effect4Chance
    {
        get => GetUInt32(7, 7, 7);
        set => SetUInt32(7, 7, 7, value);
    }

    public uint Accuracy
    {
        get => GetUInt32(7, 19, 7);
        set => SetUInt32(7, 19, 7, value);
    }

    public MoveAnimationId StartupAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 0, 9);
        set => SetUInt32(5, 0, 9, (uint)value);
    }

    public MoveAnimationId ProjectileAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 9, 9);
        set => SetUInt32(5, 9, 9, (uint)value);
    }

    public MoveAnimationId ImpactAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 18, 9);
        set => SetUInt32(5, 18, 9, (uint)value);
    }

    /// <summary>
    /// This is either wrong or i don't understand it yet
    /// </summary>
    public MoveAnimationTargetId AnimationTarget1
    {
        get => (MoveAnimationTargetId)GetUInt32(7, 26, 3);
        set => SetUInt32(7, 26, 3, (uint)value);
    }
    /// <summary>
    /// This is either wrong or i don't understand it yet
    /// </summary>
    public MoveAnimationTargetId AnimationTarget2
    {
        get => (MoveAnimationTargetId)GetUInt32(7, 29, 3);
        set => SetUInt32(7, 29, 3, (uint)value);
    }

    public MoveMovementAnimationId MovementAnimation
    {
        get => (MoveMovementAnimationId)GetUInt32(7, 14, 5);
        set => SetUInt32(7, 14, 5, (uint)value);
    }

}
