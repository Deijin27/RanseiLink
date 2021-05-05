using Core.Models;
using Core.Enums;
using Xunit;

namespace Tests.ModelTests
{
    public class PokemonTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            var gallade = new Pokemon(new byte[]
            {
                0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
                0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69, 0xFE, 0x03, 0x18,
                0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
            });

            Assert.Equal("Gallade", gallade.Name);
            Assert.Equal(TypeId.Psychic, gallade.Type1);
            Assert.Equal(TypeId.Fighting, gallade.Type2);
            Assert.Equal(AbilityId.Parry, gallade.Ability1);
            Assert.Equal(AbilityId.Conqueror, gallade.Ability2);
            Assert.Equal(AbilityId.Justified, gallade.Ability3);
            Assert.Equal(MoveId.PsychoCut, gallade.Move);
            Assert.Equal(EvolutionConditionId.Item, gallade.EvolutionCondition1);
            Assert.Equal(ItemId.DawnStone, (ItemId)(int)gallade.QuantityForEvolutionCondition1);
            Assert.Equal(EvolutionConditionId.WarriorGender, gallade.EvolutionCondition2);
            Assert.Equal(GenderId.Male, (GenderId)(int)gallade.QuantityForEvolutionCondition2);
            Assert.Equal(246u, gallade.Hp);
            Assert.Equal(255u, gallade.Atk);
            Assert.Equal(185u, gallade.Def);
            Assert.Equal(165u, gallade.Spe);
        }
    }
}
