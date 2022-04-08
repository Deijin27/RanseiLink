using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class BaseWarrior : BaseDataWindow
{
    public const int DataLength = 0x14;
    public BaseWarrior(byte[] data) : base(data, DataLength) { }
    public BaseWarrior() : this(new byte[DataLength]) { }

    public uint Sprite
    {
        get => GetUInt32(0, 0, 8);
        set => SetUInt32(0, 0, 8, value);
    }

    public WarriorSprite2Id Sprite_Unknown
    {
        get => (WarriorSprite2Id)GetUInt32(0, 8, 8);
        set => SetUInt32(0, 8, 8, (uint)value);
    }

    public GenderId Gender
    {
        get => (GenderId)GetUInt32(0, 16, 1);
        set => SetUInt32(0, 16, 1, (uint)value);
    }

    public uint WarriorName
    {
        get => GetUInt32(0, 17, 8);
        set => SetUInt32(0, 17, 8, value);
    }

    public TypeId Speciality1
    {
        get => (TypeId)GetUInt32(1, 0, 5);
        set => SetUInt32(1, 0, 5, (uint)value);
    }

    public TypeId Speciality2
    {
        get => (TypeId)GetUInt32(1, 5, 5);
        set => SetUInt32(1, 5, 5, (uint)value);
    }

    public TypeId Weakness1
    {
        get => (TypeId)GetUInt32(1, 10, 5);
        set => SetUInt32(1, 10, 5, (uint)value);
    }

    public TypeId Weakness2
    {
        get => (TypeId)GetUInt32(1, 15, 5);
        set => SetUInt32(1, 15, 5, (uint)value);
    }

    public WarriorSkillId Skill
    {
        get => (WarriorSkillId)GetUInt32(2, 0, 7);
        set => SetUInt32(2, 0, 7, (uint)value);
    }

    public uint Power
    {
        get => GetUInt32(3, 0, 7);
        set => SetUInt32(3, 0, 7, value);
    }

    public uint Wisdom
    {
        get => GetUInt32(3, 7, 7);
        set => SetUInt32(3, 7, 7, value);
    }

    public uint Charisma
    {
        get => GetUInt32(3, 14, 7);
        set => SetUInt32(3, 14, 7, value);
    }

    public uint Capacity
    {
        get => GetUInt32(3, 21, 4);
        set => SetUInt32(3, 21, 4, value);
    }

    public RankUpConditionId RankUpCondition1
    {
        get => (RankUpConditionId)GetUInt32(2, 24, 4);
        set => SetUInt32(2, 24, 4, (uint)value);
    }

    public RankUpConditionId RankUpCondition2
    {
        get => (RankUpConditionId)GetUInt32(2, 28, 4);
        set => SetUInt32(2, 28, 4, (uint)value);
    }

    public uint Quantity1ForRankUpCondition
    {
        get => GetUInt32(4, 9, 9);
        set => SetUInt32(4, 9, 9, value);
    }

    public uint Quantity2ForRankUpCondition
    {
        get => GetUInt32(4, 18, 9);
        set => SetUInt32(4, 18, 9, value);
    }

    public WarriorId RankUp
    {
        get => (WarriorId)GetUInt32(2, 15, 9);
        set => SetUInt32(2, 15, 9, (uint)value);
    }

    public PokemonId RankUpPokemon1
    {
        get => (PokemonId)GetUInt32(1, 20, 9);
        set => SetUInt32(1, 20, 9, (uint)value);
    }

    public PokemonId RankUpPokemon2
    {
        get => (PokemonId)GetUInt32(4, 0, 9);
        set => SetUInt32(4, 0, 9, (uint)value);
    }

    public uint RankUpLink
    {
        get => GetUInt32(0, 25, 7);
        set => SetUInt32(0, 25, 7, value);
    }

}
