using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.CoreTests.ModelTests;

public class ScenarioPokemonTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        ScenarioPokemon a = new ScenarioPokemon(new byte[]
        {
            0x14, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x97, 0x04
        });

        a.Pokemon.Should().Be(PokemonId.Jigglypuff);
        a.Ability.Should().Be(AbilityId.Lullaby);
        a.HpIv.Should().Be(15);
        a.AtkIv.Should().Be(15);
        a.DefIv.Should().Be(15);
        a.SpeIv.Should().Be(15);
        a.Exp.Should().Be(1020);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        ScenarioPokemon a = new ScenarioPokemon
        {
            Pokemon = PokemonId.Jigglypuff,
            Ability = AbilityId.Lullaby,
            HpIv = 15,
            AtkIv = 15,
            DefIv = 15,
            SpeIv = 15,
            Exp = 1020
        };

        var expected = new byte[]
        {
            0x14, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x97, 0x04
        };

        a.Pokemon.Should().Be(PokemonId.Jigglypuff);
        a.Ability.Should().Be(AbilityId.Lullaby);
        a.HpIv.Should().Be(15);
        a.AtkIv.Should().Be(15);
        a.DefIv.Should().Be(15);
        a.SpeIv.Should().Be(15);
        a.Exp.Should().Be(1020);

        a.Data.Should().Equal(expected);
    }
}
