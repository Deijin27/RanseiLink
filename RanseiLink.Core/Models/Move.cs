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
        get => (MoveMovementFlags)GetUInt32(3, 8, 24);
        set => SetUInt32(3, 8, 24, (uint)value);
    }

    public TypeId Type
    {
        get => (TypeId)GetUInt32(4, 5, 0);
        set => SetUInt32(4, 5, 0, (uint)value);
    }

    public uint Power
    {
        get => GetUInt32(4, 8, 5);
        set => SetUInt32(4, 8, 5, value);
    }

    public MoveEffectId Effect1
    {
        get => (MoveEffectId)GetUInt32(4, 7, 13);
        set => SetUInt32(4, 7, 13, (uint)value);
    }

    public uint Effect1Chance
    {
        get => GetUInt32(4, 7, 20);
        set => SetUInt32(4, 7, 20, value);
    }

    public MoveRangeId Range
    {
        get => (MoveRangeId)GetUInt32(4, 5, 27);
        set => SetUInt32(4, 5, 27, (uint)value);
    }

    public MoveEffectId Effect2
    {
        get => (MoveEffectId)GetUInt32(6, 7, 0);
        set => SetUInt32(6, 7, 0, (uint)value);
    }

    public uint Effect2Chance
    {
        get => GetUInt32(6, 7, 7);
        set => SetUInt32(6, 7, 7, value);
    }

    public MoveEffectId Effect3
    {
        get => (MoveEffectId)GetUInt32(6, 7, 14);
        set => SetUInt32(6, 7, 14, (uint)value);
    }

    public uint Effect3Chance
    {
        get => GetUInt32(6, 7, 21);
        set => SetUInt32(6, 7, 21, value);
    }

    public MoveEffectId Effect4
    {
        get => (MoveEffectId)GetUInt32(7, 7, 0);
        set => SetUInt32(7, 7, 0, (uint)value);
    }

    public uint Effect4Chance
    {
        get => GetUInt32(7, 7, 7);
        set => SetUInt32(7, 7, 7, value);
    }

    public uint Accuracy
    {
        get => GetUInt32(7, 7, 19);
        set => SetUInt32(7, 7, 19, value);
    }

    public MoveAnimationId StartupAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 9, 0);
        set => SetUInt32(5, 9, 0, (uint)value);
    }

    public MoveAnimationId ProjectileAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 9, 9);
        set => SetUInt32(5, 9, 9, (uint)value);
    }

    public MoveAnimationId ImpactAnimation
    {
        get => (MoveAnimationId)GetUInt32(5, 9, 18);
        set => SetUInt32(5, 9, 18, (uint)value);
    }

    /// <summary>
    /// This is either wrong or i don't understand it yet
    /// </summary>
    public MoveAnimationTargetId AnimationTarget1
    {
        get => (MoveAnimationTargetId)GetUInt32(7, 3, 26);
        set => SetUInt32(7, 3, 26, (uint)value);
    }
    /// <summary>
    /// This is either wrong or i don't understand it yet
    /// </summary>
    public MoveAnimationTargetId AnimationTarget2
    {
        get => (MoveAnimationTargetId)GetUInt32(7, 3, 29);
        set => SetUInt32(7, 3, 29, (uint)value);
    }

    public MoveMovementAnimationId MovementAnimation
    {
        get => (MoveMovementAnimationId)GetUInt32(7, 5, 14);
        set => SetUInt32(7, 5, 14, (uint)value);
    }

}
