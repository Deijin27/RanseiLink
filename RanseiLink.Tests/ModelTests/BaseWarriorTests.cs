using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using Xunit;

namespace RanseiLink.Tests.ModelTests
{
    public class BaseWarriorTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IBaseWarrior a = new BaseWarrior(new byte[]
            {
                0x17, 0x0A, 0xED, 0x78,
                0x27, 0x29, 0x76, 0x01,
                0x83, 0x25, 0x0C, 0x90,
                0x41, 0xE7, 0xB2, 0x14,
                0x18, 0x0E, 0xFC, 0x07
            });

            Assert.Equal(WarriorSpriteId.Nene_1, a.Sprite);
            Assert.Equal(GenderId.Female, a.Gender);
            Assert.Equal(118u, a.WarriorName);
            Assert.Equal(TypeId.Poison, a.Speciality1);
            Assert.Equal(TypeId.Flying, a.Speciality2);
            Assert.Equal(TypeId.Psychic, a.Weakness1);
            Assert.Equal(TypeId.Rock, a.Weakness2);
            Assert.Equal(WarriorSkillId.Rally, a.Skill);
            Assert.Equal(65u, a.Power);
            Assert.Equal(78u, a.Wisdom);
            Assert.Equal(75u, a.Charisma);
            Assert.Equal(5u, a.Capacity);
            Assert.Equal(WarriorId.Nene_2, a.RankUp);
            Assert.Equal(RankUpConditionId.Unknown, a.RankUpCondition1);
            Assert.Equal(RankUpConditionId.MonotypeGallery, a.RankUpCondition2);
            Assert.Equal(TypeId.Poison, (TypeId)a.Quantity1ForRankUpCondition);
            Assert.Equal(511u, a.Quantity2ForRankUpCondition);
            Assert.Equal(PokemonId.Golbat, a.RankUpPokemon1);
            Assert.Equal(PokemonId.Crobat, a.RankUpPokemon2);
            Assert.Equal(60u, a.RankUpLink);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {

            IBaseWarrior a = new BaseWarrior
            {
                Sprite = WarriorSpriteId.Naka,
                Gender = GenderId.Male,
                WarriorName = 4u,
                Speciality1 = TypeId.Fire,
                Speciality2 = TypeId.Water,
                Weakness1 = TypeId.Dark,
                Weakness2 = TypeId.Fighting,
                Skill = WarriorSkillId.Bewilder,
                Power = 3u,
                Wisdom = 24u,
                Charisma = 68u,
                Capacity = 2u,
                RankUp = WarriorId.Nobunaga_1,
                RankUpCondition1 = RankUpConditionId.AtLeastNGalleryPokemon,
                RankUpCondition2 = RankUpConditionId.WarriorInSameKingdom,
                Quantity1ForRankUpCondition = 34u,
                Quantity2ForRankUpCondition = 17u,
                RankUpPokemon1 = PokemonId.Beedrill,
                RankUpPokemon2 = PokemonId.Blitzle,
                RankUpLink = 20u,
            };

            Assert.Equal(WarriorSpriteId.Naka, a.Sprite);
            Assert.Equal(GenderId.Male, a.Gender);
            Assert.Equal(4u, a.WarriorName);
            Assert.Equal(TypeId.Fire, a.Speciality1);
            Assert.Equal(TypeId.Water, a.Speciality2);
            Assert.Equal(TypeId.Dark, a.Weakness1);
            Assert.Equal(TypeId.Fighting, a.Weakness2);
            Assert.Equal(WarriorSkillId.Bewilder, a.Skill);
            Assert.Equal(3u, a.Power);
            Assert.Equal(24u, a.Wisdom);
            Assert.Equal(68u, a.Charisma);
            Assert.Equal(2u, a.Capacity);
            Assert.Equal(WarriorId.Nobunaga_1, a.RankUp);
            Assert.Equal(RankUpConditionId.AtLeastNGalleryPokemon, a.RankUpCondition1);
            Assert.Equal(RankUpConditionId.WarriorInSameKingdom, a.RankUpCondition2);
            Assert.Equal(34u, a.Quantity1ForRankUpCondition);
            Assert.Equal(17u, a.Quantity2ForRankUpCondition);
            Assert.Equal(PokemonId.Beedrill, a.RankUpPokemon1);
            Assert.Equal(PokemonId.Blitzle, a.RankUpPokemon2);
            Assert.Equal(20u, a.RankUpLink);
        }
    }
}
