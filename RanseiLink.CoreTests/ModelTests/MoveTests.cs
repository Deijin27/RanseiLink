using Xunit;
using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.CoreTests.ModelTests;

public class MoveTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Move m = new Move(new byte[]
        {
                0x43, 0x72, 0x6F, 0x73, 0x73, 0x20, 0x50, 0x6F, 0x69, 0x73, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00,
                0xC7, 0xE4, 0xA4, 0x50, 0xFF, 0xFE, 0x5D, 0x01, 0x09, 0xC0, 0x49, 0x81, 0x09, 0x00, 0x24, 0xF3,
                0xA9, 0x42, 0x04, 0x00
        });

        Assert.Equal("Cross Poison", m.Name);
        Assert.Equal((MoveMovementFlags)0, m.MovementFlags);
        Assert.Equal(TypeId.Poison, m.Type);
        Assert.Equal(38, m.Power);
        Assert.Equal(100, m.Accuracy);
        Assert.Equal(MoveRangeId.Cross, m.Range);
        Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect1);
        Assert.Equal(10, m.Effect1Chance);
        Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect2);
        Assert.Equal(0, m.Effect2Chance);
        Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect3);
        Assert.Equal(10, m.Effect3Chance);
        Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect4);
        Assert.Equal(0, m.Effect4Chance);
        Assert.Equal(MoveMovementAnimationId.Default, m.MovementAnimation);
        Assert.Equal(MoveAnimationId.Default, m.StartupAnimation);
        Assert.Equal(MoveAnimationId.Default, m.ProjectileAnimation);
        Assert.Equal(MoveAnimationId.WhiteImpactBubble, m.ImpactAnimation);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Move m = new Move()
        {
            Name = "Cross Poison",
            MovementFlags = (MoveMovementFlags)0,
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

        Assert.Equal("Cross Poison", m.Name);
        Assert.Equal((MoveMovementFlags)0, m.MovementFlags);
        Assert.Equal(TypeId.Poison, m.Type);
        Assert.Equal(38, m.Power);
        Assert.Equal(100, m.Accuracy);
        Assert.Equal(MoveRangeId.Cross, m.Range);
        Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect1);
        Assert.Equal(10, m.Effect1Chance);
        Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect2);
        Assert.Equal(5, m.Effect2Chance);
        Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect3);
        Assert.Equal(10, m.Effect3Chance);
        Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect4);
        Assert.Equal(8, m.Effect4Chance);
        Assert.Equal(MoveMovementAnimationId.FieryDance, m.MovementAnimation);
        Assert.Equal(MoveAnimationId.OrangeOrbBurst, m.StartupAnimation);
        Assert.Equal(MoveAnimationId.OrbBlackHole, m.ProjectileAnimation);
        Assert.Equal(MoveAnimationId.PurpleCrescentBubble, m.ImpactAnimation);

        // Add Array equal test when possible
    }
}
