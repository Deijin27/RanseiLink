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

    public int Effect1Amount
    {
        get => GetInt(4, 24, 8);
        set => SetInt(4, 24, 8, value);
    }

    public WarriorSkillEffectId Effect1
    {
        get => (WarriorSkillEffectId)GetInt(5, 0, 7);
        set => SetInt(5, 0, 7, (int)value);
    }

    public WarriorSkillEffectId Effect2
    {
        get => (WarriorSkillEffectId)GetInt(5, 7, 7);
        set => SetInt(5, 7, 7, (int)value);
    }

    public int Effect2Amount
    {
        get => GetInt(5, 14, 8);
        set => SetInt(5, 14, 8, value);
    }

    public WarriorSkillEffectId Effect3
    {
        get => (WarriorSkillEffectId)GetInt(5, 22, 7);
        set => SetInt(5, 22, 7, (int)value);
    }

    public int Duration
    {
        get => GetInt(5, 29, 3);
        set => SetInt(5, 29, 3, value);
    }

    public int Effect3Amount
    {
        get => GetInt(6, 0, 8);
        set => SetInt(6, 0, 8, value);
    }

    public WarriorSkillTargetId Target
    {
        get => (WarriorSkillTargetId)GetInt(6, 8, 3);
        set => SetInt(6, 8, 3, (int)value);
    }

    /// <summary>
    /// Seems to be unused.
    /// </summary>
    public MoveAnimationId Animation
    {
        get => (MoveAnimationId)GetInt(6, 11, 9);
        set => SetInt(6, 11, 9, (int)value);
    }

}
