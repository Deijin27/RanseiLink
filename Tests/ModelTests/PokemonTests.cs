using Core.Models;
using Core.Enums;
using Xunit;
using Core;
using Core.Models.Interfaces;

namespace Tests.ModelTests
{
    public class PokemonTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IPokemon p = new Pokemon(new byte[]
            {
                0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
                0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69, 0xFE, 0x03, 0x18,
                0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
            });

            Assert.Equal("Gallade", p.Name);
            Assert.Equal(TypeId.Psychic, p.Type1);
            Assert.Equal(TypeId.Fighting, p.Type2);
            Assert.Equal(AbilityId.Parry, p.Ability1);
            Assert.Equal(AbilityId.Conqueror, p.Ability2);
            Assert.Equal(AbilityId.Justified, p.Ability3);
            Assert.Equal(MoveId.PsychoCut, p.Move);
            Assert.Equal(EvolutionConditionId.Item, p.EvolutionCondition1);
            Assert.Equal(ItemId.DawnStone, (ItemId)p.QuantityForEvolutionCondition1);
            Assert.Equal(EvolutionConditionId.WarriorGender, p.EvolutionCondition2);
            Assert.Equal(GenderId.Male, (GenderId)p.QuantityForEvolutionCondition2);
            Assert.Equal(246u, p.Hp);
            Assert.Equal(255u, p.Atk);
            Assert.Equal(185u, p.Def);
            Assert.Equal(165u, p.Spe);
            Assert.False(p.IsLegendary);
            Assert.Equal(0x40u, p.NameOrderIndex);
            Assert.Equal(475u, p.NationalPokedexNumber);
            foreach (LocationId location in EnumUtil.GetValues<LocationId>())
            {
                Assert.False(p.GetEncounterable(location, false));
            }
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IPokemon p = new Pokemon
            {
                Name = "Gallade",
                Type1 = TypeId.Psychic,
                Type2 = TypeId.Fighting,
                Ability1 = AbilityId.Parry,
                Ability2 = AbilityId.Conqueror,
                Ability3 = AbilityId.Justified,
                Move = MoveId.PsychoCut,
                EvolutionCondition1 = EvolutionConditionId.Item,
                QuantityForEvolutionCondition1 = (uint)ItemId.DawnStone,
                EvolutionCondition2 = EvolutionConditionId.WarriorGender,
                QuantityForEvolutionCondition2 = (uint)GenderId.Male,
                Hp = 246u,
                Atk = 255u,
                Def = 185u,
                Spe = 165u,
                IsLegendary = true,
                NameOrderIndex = 0x40,
                NationalPokedexNumber = 475,
            };
            p.SetEncounterable(LocationId.Aurora, true, true);
            p.SetEncounterable(LocationId.Cragspur, false, true);
            p.SetEncounterable(LocationId.Chrysalia, true, false);
            p.SetEncounterable(LocationId.Illusio, false, false);

            Assert.Equal("Gallade", p.Name);
            Assert.Equal(TypeId.Psychic, p.Type1);
            Assert.Equal(TypeId.Fighting, p.Type2);
            Assert.Equal(AbilityId.Parry, p.Ability1);
            Assert.Equal(AbilityId.Conqueror, p.Ability2);
            Assert.Equal(AbilityId.Justified, p.Ability3);
            Assert.Equal(MoveId.PsychoCut, p.Move);
            Assert.Equal(EvolutionConditionId.Item, p.EvolutionCondition1);
            Assert.Equal(ItemId.DawnStone, (ItemId)p.QuantityForEvolutionCondition1);
            Assert.Equal(EvolutionConditionId.WarriorGender, p.EvolutionCondition2);
            Assert.Equal(GenderId.Male, (GenderId)p.QuantityForEvolutionCondition2);
            Assert.Equal(246u, p.Hp);
            Assert.Equal(255u, p.Atk);
            Assert.Equal(185u, p.Def);
            Assert.Equal(165u, p.Spe);
            Assert.True(p.IsLegendary);
            Assert.Equal(0x40u, p.NameOrderIndex);
            Assert.Equal(475u, p.NationalPokedexNumber);

            Assert.True(p.GetEncounterable(LocationId.Aurora, true));
            Assert.True(p.GetEncounterable(LocationId.Cragspur, false));
            Assert.False(p.GetEncounterable(LocationId.Chrysalia, true));
            Assert.False(p.GetEncounterable(LocationId.Illusio, false));

            // Add Array equal test when possible
        }
    }
}
