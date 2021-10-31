using Xunit;
using Core.Models;
using Core.Enums;
using Core.Models.Interfaces;

namespace Tests.ModelTests
{
    public class MoveTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IMove m = new Move(new byte[]
            {
                0x43, 0x72, 0x6F, 0x73, 0x73, 0x20, 0x50, 0x6F, 0x69, 0x73, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00,
                0xC7, 0xE4, 0xA4, 0x50, 0xFF, 0xFE, 0x5D, 0x01, 0x09, 0xC0, 0x49, 0x81, 0x09, 0x00, 0x24, 0xF3,
                0xA9, 0x42, 0x04, 0x00
            });

            Assert.Equal("Cross Poison", m.Name);
            Assert.Equal((MoveMovementFlags)0, m.MovementFlags);
            Assert.Equal(TypeId.Poison, m.Type);
            Assert.Equal(38u, m.Power);
            Assert.Equal(100u, m.Accuracy);
            Assert.Equal(MoveRangeId.Cross, m.Range);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect1);
            Assert.Equal(10u, m.Effect1Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect2);
            Assert.Equal(0u, m.Effect2Chance);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect3);
            Assert.Equal(10u, m.Effect3Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect4);
            Assert.Equal(0u, m.Effect4Chance);
            Assert.Equal(MoveMovementAnimationId.Default, m.MovementAnimation);
            Assert.Equal(MoveAnimationId.Default, m.StartupAnimation);
            Assert.Equal(MoveAnimationId.Default, m.ProjectileAnimation);
            Assert.Equal(MoveAnimationId.WhiteImpactBubble, m.ImpactAnimation);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IMove m = new Move()
            {
                Name = "Cross Poison",
                MovementFlags = (MoveMovementFlags)0,
                Type = TypeId.Poison,
                Power = 38u,
                Accuracy = 100u,
                Range = MoveRangeId.Cross,
                Effect1 = MoveEffectId.ChanceToPoisonTarget,
                Effect1Chance = 10u,
                Effect2 = MoveEffectId.HighCriticalHitChance,
                Effect2Chance = 5u,
                Effect3 = MoveEffectId.ChanceToPoisonTarget,
                Effect3Chance = 10u,
                Effect4 = MoveEffectId.HighCriticalHitChance,
                Effect4Chance = 8u,
                MovementAnimation = MoveMovementAnimationId.FieryDance,
                StartupAnimation = MoveAnimationId.OrangeOrbBurst,
                ProjectileAnimation = MoveAnimationId.OrbBlackHole,
                ImpactAnimation = MoveAnimationId.PurpleCrescentBubble,
            };

            Assert.Equal("Cross Poison", m.Name);
            Assert.Equal((MoveMovementFlags)0, m.MovementFlags);
            Assert.Equal(TypeId.Poison, m.Type);
            Assert.Equal(38u, m.Power);
            Assert.Equal(100u, m.Accuracy);
            Assert.Equal(MoveRangeId.Cross, m.Range);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect1);
            Assert.Equal(10u, m.Effect1Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect2);
            Assert.Equal(5u, m.Effect2Chance);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect3);
            Assert.Equal(10u, m.Effect3Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect4);
            Assert.Equal(8u, m.Effect4Chance);
            Assert.Equal(MoveMovementAnimationId.FieryDance, m.MovementAnimation);
            Assert.Equal(MoveAnimationId.OrangeOrbBurst, m.StartupAnimation);
            Assert.Equal(MoveAnimationId.OrbBlackHole, m.ProjectileAnimation);
            Assert.Equal(MoveAnimationId.PurpleCrescentBubble, m.ImpactAnimation);

            // Add Array equal test when possible
        }
    }
}
