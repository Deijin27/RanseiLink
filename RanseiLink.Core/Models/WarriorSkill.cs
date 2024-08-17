using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class WarriorSkill : BaseDataWindow
{
    public static int DataLength(ConquestGameCode culture) => culture == ConquestGameCode.VPYJ ? 0x18 : 0x1C;
    private readonly int _cultureNameLength;
    private readonly int _cultureBinOffset;
    public WarriorSkill(byte[] data, ConquestGameCode culture) : base(data, DataLength(culture)) 
    {
        // the name has 4 fewer bytes in the japanese version of the game
        _cultureNameLength = culture == ConquestGameCode.VPYJ ? 0xE : 0x12;
        _cultureBinOffset = culture == ConquestGameCode.VPYJ ? 0 : 1;
    }
    public WarriorSkill(ConquestGameCode culture) : this(new byte[DataLength(culture)], culture) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, _cultureNameLength);
        set => SetPaddedUtf8String(0, _cultureNameLength, value);
    }

    public int Effect1Amount
    {
        get => GetInt(3 + _cultureBinOffset, 24, 8);
        set => SetInt(3 + _cultureBinOffset, 24, 8, value);
    }

    public WarriorSkillEffectId Effect1
    {
        get => (WarriorSkillEffectId)GetInt(4 + _cultureBinOffset, 0, 7);
        set => SetInt(4 + _cultureBinOffset, 0, 7, (int)value);
    }

    public WarriorSkillEffectId Effect2
    {
        get => (WarriorSkillEffectId)GetInt(4 + _cultureBinOffset, 7, 7);
        set => SetInt(4 + _cultureBinOffset, 7, 7, (int)value);
    }

    public int Effect2Amount
    {
        get => GetInt(4 + _cultureBinOffset, 14, 8);
        set => SetInt(4 + _cultureBinOffset, 14, 8, value);
    }

    public WarriorSkillEffectId Effect3
    {
        get => (WarriorSkillEffectId)GetInt(4 + _cultureBinOffset, 22, 7);
        set => SetInt(4 + _cultureBinOffset, 22, 7, (int)value);
    }

    public int Duration
    {
        get => GetInt(4 + _cultureBinOffset, 29, 3);
        set => SetInt(4 + _cultureBinOffset, 29, 3, value);
    }

    public int Effect3Amount
    {
        get => GetInt(5 + _cultureBinOffset, 0, 8);
        set => SetInt(5 + _cultureBinOffset, 0, 8, value);
    }

    public WarriorSkillTargetId Target
    {
        get => (WarriorSkillTargetId)GetInt(5 + _cultureBinOffset, 8, 3);
        set => SetInt(5 + _cultureBinOffset, 8, 3, (int)value);
    }

    /// <summary>
    /// Seems to be unused.
    /// </summary>
    public MoveAnimationId Animation
    {
        get => (MoveAnimationId)GetInt(5 + _cultureBinOffset, 11, 9);
        set => SetInt(5  + _cultureBinOffset, 11, 9, (int)value);
    }

}