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
        Assert.False(p.ScenarioPokemonIsDefault(0));
        Assert.Equal(WarriorClassId.ArmyMember, p.Class);
        Assert.Equal(KingdomId.Default, p.Kingdom);
        Assert.Equal(17u, p.Army);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        IScenarioWarrior p = new ScenarioWarrior
        {
            Warrior = WarriorId.Nobuchika,
            Class = WarriorClassId.FreeWarrior,
            Kingdom = KingdomId.Valora,
            Army = 5u
        };
        p.ScenarioPokemon = 57;

        Assert.Equal(WarriorId.Nobuchika, p.Warrior);
        Assert.Equal(57u, p.ScenarioPokemon);
        Assert.Equal(WarriorClassId.FreeWarrior, p.Class);
        Assert.Equal(KingdomId.Valora, p.Kingdom);
        Assert.Equal(5u, p.Army);
        // Add Array equal test when possible
    }
}
