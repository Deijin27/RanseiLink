using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class WarriorSkill : BaseDataWindow
{
    public const int DataLength = 0x1C;
    public WarriorSkill(byte[] data) : base(data, DataLength) { }
    public WarriorSkill() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 15);
        set => SetPaddedUtf8String(0, 15, value);
    }

    public uint Effect1Amount
    {
        get => GetUInt32(4, 8, 24);
        set => SetUInt32(4, 8, 24, value);
    }

    public WarriorSkillEffectId Effect1
    {
        get => (WarriorSkillEffectId)GetUInt32(5, 7, 0);
        set => SetUInt32(5, 7, 0, (uint)value);
    }

    public WarriorSkillEffectId Effect2
    {
        get => (WarriorSkillEffectId)GetUInt32(5, 7, 7);
        set => SetUInt32(5, 7, 7, (uint)value);
    }

    public uint Effect2Amount
    {
        get => GetUInt32(5, 8, 14);
        set => SetUInt32(5, 8, 14, value);
    }

    public WarriorSkillEffectId Effect3
    {
        get => (WarriorSkillEffectId)GetUInt32(5, 7, 22);
        set => SetUInt32(5, 7, 22, (uint)value);
    }

    public uint Duration
    {
        get => GetUInt32(5, 3, 29);
        set => SetUInt32(5, 3, 29, value);
    }

    public uint Effect3Amount
    {
        get => GetUInt32(6, 8, 0);
        set => SetUInt32(6, 8, 0, value);
    }

    public WarriorSkillTargetId Target
    {
        get => (WarriorSkillTargetId)GetUInt32(6, 3, 8);
        set => SetUInt32(6, 3, 8, (uint)value);
    }

    /// <summary>
    /// Seems to be unused.
    /// </summary>
    public MoveAnimationId Animation
    {
        get => (MoveAnimationId)GetUInt32(6, 9, 11);
        set => SetUInt32(6, 9, 11, (uint)value);
    }

}
