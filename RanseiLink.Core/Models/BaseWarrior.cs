using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class BaseWarrior : BaseDataWindow
{
    public const int DataLength = 0x14;
    public BaseWarrior(byte[] data) : base(data, DataLength) { }
    public BaseWarrior() : this(new byte[DataLength]) { }

    public int Sprite
    {
        get => GetInt(0, 0, 8);
        set => SetInt(0, 0, 8, value);
    }

    public SpeakerId SpeakerId
    {
        get => (SpeakerId)GetInt(0, 8, 8);
        set => SetInt(0, 8, 8, (int)value);
    }

    public GenderId Gender
    {
        get => (GenderId)GetInt(0, 16, 1);
        set => SetInt(0, 16, 1, (int)value);
    }

    public int WarriorName
    {
        get => GetInt(0, 17, 8);
        set => SetInt(0, 17, 8, value);
    }

    public TypeId Speciality1
    {
        get => (TypeId)GetInt(1, 0, 5);
        set => SetInt(1, 0, 5, (int)value);
    }

    public TypeId Speciality2
    {
        get => (TypeId)GetInt(1, 5, 5);
        set => SetInt(1, 5, 5, (int)value);
    }

    public TypeId Weakness1
    {
        get => (TypeId)GetInt(1, 10, 5);
        set => SetInt(1, 10, 5, (int)value);
    }

    public TypeId Weakness2
    {
        get => (TypeId)GetInt(1, 15, 5);
        set => SetInt(1, 15, 5, (int)value);
    }

    public WarriorSkillId Skill
    {
        get => (WarriorSkillId)GetInt(2, 0, 7);
        set => SetInt(2, 0, 7, (int)value);
    }

    public int Power
    {
        get => GetInt(3, 0, 7);
        set => SetInt(3, 0, 7, value);
    }

    public int Wisdom
    {
        get => GetInt(3, 7, 7);
        set => SetInt(3, 7, 7, value);
    }

    public int Charisma
    {
        get => GetInt(3, 14, 7);
        set => SetInt(3, 14, 7, value);
    }

    public int Capacity
    {
        get => GetInt(3, 21, 4);
        set => SetInt(3, 21, 4, value);
    }

    public RankUpConditionId RankUpCondition1
    {
        get => (RankUpConditionId)GetInt(2, 24, 4);
        set => SetInt(2, 24, 4, (int)value);
    }

    public RankUpConditionId RankUpCondition2
    {
        get => (RankUpConditionId)GetInt(2, 28, 4);
        set => SetInt(2, 28, 4, (int)value);
    }

    public int Quantity1ForRankUpCondition
    {
        get => GetInt(4, 9, 9);
        set => SetInt(4, 9, 9, value);
    }

    public int Quantity2ForRankUpCondition
    {
        get => GetInt(4, 18, 9);
        set => SetInt(4, 18, 9, value);
    }

    public WarriorId RankUp
    {
        get => (WarriorId)GetInt(2, 15, 9);
        set => SetInt(2, 15, 9, (int)value);
    }

    public PokemonId RankUpPokemon1
    {
        get => (PokemonId)GetInt(1, 20, 9);
        set => SetInt(1, 20, 9, (int)value);
    }

    public PokemonId RankUpPokemon2
    {
        get => (PokemonId)GetInt(4, 0, 9);
        set => SetInt(4, 0, 9, (int)value);
    }

    public int RankUpLink
    {
        get => GetInt(0, 25, 7);
        set => SetInt(0, 25, 7, value);
    }

}