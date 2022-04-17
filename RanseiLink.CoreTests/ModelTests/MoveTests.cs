using Xunit;
using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;
using FluentAssertions;

namespace RanseiLink.CoreTests.ModelTests;

public class MoveTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Move a = new Move(new byte[]
        {
                0x43, 0x72, 0x6F, 0x73, 0x73, 0x20, 0x50, 0x6F, 0x69, 0x73, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00,
                0xC7, 0xE4, 0xA4, 0x50, 0xFF, 0xFE, 0x5D, 0x01, 0x09, 0xC0, 0x49, 0x81, 0x09, 0x00, 0x24, 0xF3,
                0xA9, 0x42, 0x04, 0x00
        });

        a.Name.Should().Be("Cross Poison");
        a.MovementFlags.Should().Be(0);
        a.Type.Should().Be(TypeId.Poison);
        a.Power.Should().Be(38);
        a.Accuracy.Should().Be(100);
        a.Range.Should().Be(MoveRangeId.Cross);
        a.Effect1.Should().Be(MoveEffectId.ChanceToPoisonTarget);
        a.Effect1Chance.Should().Be(10);
        a.Effect2.Should().Be(MoveEffectId.HighCriticalHitChance);
        a.Effect2Chance.Should().Be(0);
        a.Effect3.Should().Be(MoveEffectId.ChanceToPoisonTarget);
        a.Effect3Chance.Should().Be(10);
        a.Effect4.Should().Be(MoveEffectId.HighCriticalHitChance);
        a.Effect4Chance.Should().Be(0);
        a.MovementAnimation.Should().Be(MoveMovementAnimationId.Default);
        a.StartupAnimation.Should().Be(MoveAnimationId.Default);
        a.ProjectileAnimation.Should().Be(MoveAnimationId.Default);
        a.ImpactAnimation.Should().Be(MoveAnimationId.WhiteImpactBubble);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Move a = new Move()
        {
            Name = "Cross Poison",
            MovementFlags = 0,
            Type = TypeId.Poison,
            Power = 38,
            Accuracy = 100,
            Range = MoveRangeId.Cross,
            Effect1 = MoveEffectId.ChanceToPoisonTarget,
            Effect1Chance = 10,
            Effect2 = MoveEffectId.HighCriticalHitChance,
            Effect2Chance = 5,
            Effect3 = MoveEffectId.ChanceToPoisonTarget,
            Effect3Chance = 10,
            Effect4 = MoveEffectId.HighCriticalHitChance,
            Effect4Chance = 8,
            MovementAnimation = MoveMovementAnimationId.FieryDance,
            StartupAnimation = MoveAnimationId.OrangeOrbBurst,
            ProjectileAnimation = MoveAnimationId.OrbBlackHole,
            ImpactAnimation = MoveAnimationId.PurpleCrescentBubble,
        };

        a.Name.Should().Be("Cross Poison");
        a.MovementFlags.Should().Be(0);
        a.Type.Should().Be(TypeId.Poison);
        a.Power.Should().Be(38);
        a.Accuracy.Should().Be(100);
        a.Range.Should().Be(MoveRangeId.Cross);
        a.Effect1.Should().Be(MoveEffectId.ChanceToPoisonTarget);
        a.Effect1Chance.Should().Be(10);
        a.Effect2.Should().Be(MoveEffectId.HighCriticalHitChance);
        a.Effect2Chance.Should().Be(5);
        a.Effect3.Should().Be(MoveEffectId.ChanceToPoisonTarget);
        a.Effect3Chance.Should().Be(10);
        a.Effect4.Should().Be(MoveEffectId.HighCriticalHitChance);
        a.Effect4Chance.Should().Be(8);
        a.MovementAnimation.Should().Be(MoveMovementAnimationId.FieryDance);
        a.StartupAnimation.Should().Be(MoveAnimationId.OrangeOrbBurst);
        a.ProjectileAnimation.Should().Be(MoveAnimationId.OrbBlackHole);
        a.ImpactAnimation.Should().Be(MoveAnimationId.PurpleCrescentBubble);

        // Add Array equal test when possible
    }
}
