using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class BaseWarriorTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        BaseWarrior a = new BaseWarrior(new byte[]
        {
                0x17, 0x0A, 0xED, 0x78,
                0x27, 0x29, 0x76, 0x01,
                0x83, 0x25, 0x0C, 0x90,
                0x41, 0xE7, 0xB2, 0x14,
                0x18, 0x0E, 0xFC, 0x07
        });

        Assert.Equal(23, a.Sprite);
        Assert.Equal(GenderId.Female, a.Gender);
        Assert.Equal(118, a.WarriorName);
        Assert.Equal(TypeId.Poison, a.Speciality1);
        Assert.Equal(TypeId.Flying, a.Speciality2);
        Assert.Equal(TypeId.Psychic, a.Weakness1);
        Assert.Equal(TypeId.Rock, a.Weakness2);
        Assert.Equal(WarriorSkillId.Rally, a.Skill);
        Assert.Equal(65, a.Power);
        Assert.Equal(78, a.Wisdom);
        Assert.Equal(75, a.Charisma);
        Assert.Equal(5, a.Capacity);
        Assert.Equal(WarriorId.Nene_2, a.RankUp);
        Assert.Equal(RankUpConditionId.Unknown, a.RankUpCondition1);
        Assert.Equal(RankUpConditionId.MonotypeGallery, a.RankUpCondition2);
        Assert.Equal(TypeId.Poison, (TypeId)a.Quantity1ForRankUpCondition);
        Assert.Equal(511, a.Quantity2ForRankUpCondition);
        Assert.Equal(PokemonId.Golbat, a.RankUpPokemon1);
        Assert.Equal(PokemonId.Crobat, a.RankUpPokemon2);
        Assert.Equal(60, a.RankUpLink);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {

        BaseWarrior a = new BaseWarrior
        {
            Sprite = 128,
            Gender = GenderId.Male,
            WarriorName = 4,
            Speciality1 = TypeId.Fire,
            Speciality2 = TypeId.Water,
            Weakness1 = TypeId.Dark,
            Weakness2 = TypeId.Fighting,
            Skill = WarriorSkillId.Bewilder,
            Power = 3,
            Wisdom = 24,
            Charisma = 68,
            Capacity = 2,
            RankUp = WarriorId.Nobunaga_1,
            RankUpCondition1 = RankUpConditionId.AtLeastNGalleryPokemon,
            RankUpCondition2 = RankUpConditionId.WarriorInSameKingdom,
            Quantity1ForRankUpCondition = 34,
            Quantity2ForRankUpCondition = 17,
            RankUpPokemon1 = PokemonId.Beedrill,
            RankUpPokemon2 = PokemonId.Blitzle,
            RankUpLink = 20,
        };

        Assert.Equal(128, a.Sprite);
        Assert.Equal(GenderId.Male, a.Gender);
        Assert.Equal(4, a.WarriorName);
        Assert.Equal(TypeId.Fire, a.Speciality1);
        Assert.Equal(TypeId.Water, a.Speciality2);
        Assert.Equal(TypeId.Dark, a.Weakness1);
        Assert.Equal(TypeId.Fighting, a.Weakness2);
        Assert.Equal(WarriorSkillId.Bewilder, a.Skill);
        Assert.Equal(3, a.Power);
        Assert.Equal(24, a.Wisdom);
        Assert.Equal(68, a.Charisma);
        Assert.Equal(2, a.Capacity);
        Assert.Equal(WarriorId.Nobunaga_1, a.RankUp);
        Assert.Equal(RankUpConditionId.AtLeastNGalleryPokemon, a.RankUpCondition1);
        Assert.Equal(RankUpConditionId.WarriorInSameKingdom, a.RankUpCondition2);
        Assert.Equal(34, a.Quantity1ForRankUpCondition);
        Assert.Equal(17, a.Quantity2ForRankUpCondition);
        Assert.Equal(PokemonId.Beedrill, a.RankUpPokemon1);
        Assert.Equal(PokemonId.Blitzle, a.RankUpPokemon2);
        Assert.Equal(20, a.RankUpLink);
    }
}
