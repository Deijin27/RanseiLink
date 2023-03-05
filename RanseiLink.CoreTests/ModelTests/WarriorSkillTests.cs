using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ModelTests;

public class WarriorSkillTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        WarriorSkill a = new WarriorSkill(new byte[]
        {
                0x4D, 0x61, 0x79, 0x68,
                0x65, 0x6D, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x01,
                0x85, 0x09, 0x80, 0x66,
                0x28, 0xB8, 0x06, 0x00
        });

        a.Name.Should().Be("Mayhem");
        a.Effect1Amount.Should().Be(1);
        a.Effect1.Should().Be(WarriorSkillEffectId.RaiseRange);
        a.Effect2.Should().Be(WarriorSkillEffectId.ClimbHigher);
        a.Effect2Amount.Should().Be(0);
        a.Effect3.Should().Be(WarriorSkillEffectId.ChanceToFlinchOpponent);
        a.Effect3Amount.Should().Be(40);
        a.Target.Should().Be(WarriorSkillTargetId.Self);
        a.Duration.Should().Be(3);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        WarriorSkill a = new WarriorSkill()
        {
            Name = "Mayhem",
            Effect1Amount = 1,
            Effect1 = WarriorSkillEffectId.RaiseRange,
            Effect2 = WarriorSkillEffectId.ClimbHigher,
            Effect2Amount = 3,
            Effect3 = WarriorSkillEffectId.ChanceToFlinchOpponent,
            Effect3Amount = 40,
            Target = WarriorSkillTargetId.SelfAndAllAllies,
            Duration = 3,
        };

        a.Name.Should().Be("Mayhem");
        a.Effect1Amount.Should().Be(1);
        a.Effect1.Should().Be(WarriorSkillEffectId.RaiseRange);
        a.Effect2.Should().Be(WarriorSkillEffectId.ClimbHigher);
        a.Effect2Amount.Should().Be(3);
        a.Effect3.Should().Be(WarriorSkillEffectId.ChanceToFlinchOpponent);
        a.Effect3Amount.Should().Be(40);
        a.Target.Should().Be(WarriorSkillTargetId.SelfAndAllAllies);
        a.Duration.Should().Be(3);

        // Add Array equal test when possible
    }
}
