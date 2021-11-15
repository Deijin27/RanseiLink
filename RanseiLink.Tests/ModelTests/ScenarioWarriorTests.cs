using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using Xunit;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Tests.ModelTests;

public class ScenarioWarriorTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        IScenarioWarrior p = new ScenarioWarrior(new byte[]
        {
                0x08, 0x12, 0x23, 0x88,
                0x00, 0x00, 0x00, 0x00,
                0x18, 0x02, 0x00, 0x69,
                0x00, 0x00, 0x01, 0x00,
                0x4C, 0x04, 0x4C, 0x04,
                0x4C, 0x04, 0x4C, 0x04,
                0x4C, 0x04, 0x4C, 0x04,
                0x4C, 0x04, 0x00, 0x00,
        });

        Assert.Equal(WarriorId.Oichi_1, p.Warrior);
        Assert.Equal(1u, p.ScenarioPokemon);
        Assert.False(p.ScenarioPokemonIsDefault);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        IScenarioWarrior p = new ScenarioWarrior
        {
            Warrior = WarriorId.Nobuchika,
            ScenarioPokemon = 57u,
        };

        Assert.Equal(WarriorId.Nobuchika, p.Warrior);
        Assert.Equal(57u, p.ScenarioPokemon);

        // Add Array equal test when possible
    }
}
