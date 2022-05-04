using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using Xunit;
using FluentAssertions;

namespace RanseiLink.CoreTests.ModelTests;

public class ScenarioWarriorTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        ScenarioWarrior a = new ScenarioWarrior(new byte[]
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

        a.Warrior.Should().Be(WarriorId.Oichi_1);
        a.GetScenarioPokemon(0).Should().Be(1);
        a.ScenarioPokemonIsDefault(0).Should().BeFalse();
        a.Class.Should().Be(WarriorClassId.ArmyMember);
        a.Kingdom.Should().Be(KingdomId.Default);
        a.Army.Should().Be(17);
        a.Item.Should().Be(ItemId.Default);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        ScenarioWarrior a = new ScenarioWarrior
        {
            Warrior = WarriorId.Nobuchika,
            Class = WarriorClassId.FreeWarrior,
            Kingdom = KingdomId.Valora,
            Army = 5,
            Item = ItemId.DoublePlay
        };
        a.SetScenarioPokemon(0, 57);

        a.Warrior.Should().Be(WarriorId.Nobuchika);
        a.GetScenarioPokemon(0).Should().Be(57);
        a.Class.Should().Be(WarriorClassId.FreeWarrior);
        a.Kingdom.Should().Be(KingdomId.Valora);
        a.Army.Should().Be(5);
        a.Item.Should().Be(ItemId.DoublePlay);

        // Add Array equal test when possible
    }
}
