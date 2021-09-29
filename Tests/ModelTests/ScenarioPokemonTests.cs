using Core.Models;
using Core.Enums;
using Xunit;
using Core.Models.Interfaces;

namespace Tests.ModelTests
{
    public class ScenarioPokemonTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IScenarioPokemon p = new ScenarioPokemon(new byte[]
            {
                0x14, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x97, 0x04
            });

            Assert.Equal(PokemonId.Jigglypuff, p.Pokemon);
            Assert.Equal(AbilityId.Lullaby, p.Ability);
            Assert.Equal(15u, p.HpIv);
            Assert.Equal(15u, p.AtkIv);
            Assert.Equal(15u, p.DefIv);
            Assert.Equal(15u, p.SpeIv);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IScenarioPokemon p = new ScenarioPokemon
            {
                Pokemon = PokemonId.Pikachu,
                Ability = AbilityId.Bodyguard,
                HpIv = 5u,
                AtkIv = 0u,
                DefIv = 30u,
                SpeIv = 16u,
            };

            Assert.Equal(PokemonId.Pikachu, p.Pokemon);
            Assert.Equal(AbilityId.Bodyguard, p.Ability);
            Assert.Equal(5u, p.HpIv);
            Assert.Equal(0u, p.AtkIv);
            Assert.Equal(30u, p.DefIv);
            Assert.Equal(16u, p.SpeIv);

            // Add Array equal test when possible
        }
    }
}
