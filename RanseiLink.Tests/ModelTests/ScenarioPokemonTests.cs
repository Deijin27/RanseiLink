using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using Xunit;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Tests.ModelTests;

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
        Assert.Equal(1020, p.Exp);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        IScenarioPokemon p = new ScenarioPokemon
        {
            Pokemon = PokemonId.Jigglypuff,
            Ability = AbilityId.Lullaby,
            HpIv = 15u,
            AtkIv = 15u,
            DefIv = 15u,
            SpeIv = 15u,
            Exp = 1020
        };

        var expected = new byte[]
        {
                0x14, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x97, 0x04
        };

        Assert.Equal(PokemonId.Jigglypuff, p.Pokemon);
        Assert.Equal(AbilityId.Lullaby, p.Ability);
        Assert.Equal(15u, p.HpIv);
        Assert.Equal(15u, p.AtkIv);
        Assert.Equal(15u, p.DefIv);
        Assert.Equal(15u, p.SpeIv);
        Assert.Equal(1020, p.Exp);

        Assert.Equal(expected, p.Data);
    }
}
