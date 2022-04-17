using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;
using FluentAssertions;

namespace RanseiLink.CoreTests.ModelTests;

public class AbilityTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Ability a = new Ability(new byte[]
        {
                0x4C, 0x61, 0x73, 0x74,
                0x20, 0x42, 0x61, 0x73,
                0x74, 0x69, 0x6F, 0x6E,
                0x00, 0x00, 0x00, 0x02,
                0x41, 0x08, 0x00, 0x00
        });

        a.Name.Should().Be("Last Bastion");
        a.Effect1Amount.Should().Be(2);
        a.Effect1.Should().Be(AbilityEffectId.IncreaseUserAttack);
        a.Effect2.Should().Be(AbilityEffectId.IncreaseUserDefence);
        a.Effect2Amount.Should().Be(2);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        var lastBastionData = new byte[]
        {
                0x4C, 0x61, 0x73, 0x74,
                0x20, 0x42, 0x61, 0x73,
                0x74, 0x69, 0x6F, 0x6E,
                0x00, 0x00, 0x00, 0x02,
                0x41, 0x08, 0x00, 0x00
        };

        Ability a = new Ability
        {
            Name = "Last Bastion",
            Effect1Amount = 2,
            Effect1 = AbilityEffectId.IncreaseUserAttack,
            Effect2 = AbilityEffectId.IncreaseUserDefence,
            Effect2Amount = 2
        };

        a.Name.Should().Be("Last Bastion");
        a.Effect1Amount.Should().Be(2);
        a.Effect1.Should().Be(AbilityEffectId.IncreaseUserAttack);
        a.Effect2.Should().Be(AbilityEffectId.IncreaseUserDefence);
        a.Effect2Amount.Should().Be(2);

        a.Data.Should().Equal(lastBastionData);
    }
}
