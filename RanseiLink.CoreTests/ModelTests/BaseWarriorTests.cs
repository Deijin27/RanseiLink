using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

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

        a.Sprite.Should().Be(23);
        a.Gender.Should().Be(GenderId.Female);
        a.Name.Should().Be(118);
        a.Speciality1.Should().Be(TypeId.Poison);
        a.Speciality2.Should().Be(TypeId.Flying);
        a.Weakness1.Should().Be(TypeId.Psychic);
        a.Weakness2.Should().Be(TypeId.Rock);
        a.Skill.Should().Be(WarriorSkillId.Rally);
        a.Power.Should().Be(65);
        a.Wisdom.Should().Be(78);
        a.Charisma.Should().Be(75);
        a.Capacity.Should().Be(5);
        a.RankUp.Should().Be(WarriorId.Nene_2);
        a.RankUpCondition1.Should().Be(RankUpConditionId.Unknown);
        a.RankUpCondition2.Should().Be(RankUpConditionId.MonotypeGallery);
        ((TypeId)a.Quantity1ForRankUpCondition).Should().Be(TypeId.Poison);
        a.Quantity2ForRankUpCondition.Should().Be(511);
        a.RankUpPokemon1.Should().Be(PokemonId.Golbat);
        a.RankUpPokemon2.Should().Be(PokemonId.Crobat);
        a.RankUpLink.Should().Be(60);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {

        BaseWarrior a = new BaseWarrior
        {
            Sprite = 128,
            Gender = GenderId.Male,
            Name = 4,
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

        a.Sprite.Should().Be(128);
        a.Gender.Should().Be(GenderId.Male);
        a.Name.Should().Be(4);
        a.Speciality1.Should().Be(TypeId.Fire);
        a.Speciality2.Should().Be(TypeId.Water);
        a.Weakness1.Should().Be(TypeId.Dark);
        a.Weakness2.Should().Be(TypeId.Fighting);
        a.Skill.Should().Be(WarriorSkillId.Bewilder);
        a.Power.Should().Be(3);
        a.Wisdom.Should().Be(24);
        a.Charisma.Should().Be(68);
        a.Capacity.Should().Be(2);
        a.RankUp.Should().Be(WarriorId.Nobunaga_1);
        a.RankUpCondition1.Should().Be(RankUpConditionId.AtLeastNGalleryPokemon);
        a.RankUpCondition2.Should().Be(RankUpConditionId.WarriorInSameKingdom);
        a.Quantity1ForRankUpCondition.Should().Be(34);
        a.Quantity2ForRankUpCondition.Should().Be(17);
        a.RankUpPokemon1.Should().Be(PokemonId.Beedrill);
        a.RankUpPokemon2.Should().Be(PokemonId.Blitzle);
        a.RankUpLink.Should().Be(20);
    }
}
