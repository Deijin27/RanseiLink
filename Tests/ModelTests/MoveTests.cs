using Xunit;
using Core.Models;
using Core.Enums;

namespace Tests.ModelTests
{
    public class MoveTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            var m = new Move(new byte[] 
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
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect0);
            Assert.Equal(10u, m.Effect0Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect1);
            Assert.Equal(0u, m.Effect1Chance);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect2);
            Assert.Equal(10u, m.Effect2Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect3);
            Assert.Equal(0u, m.Effect3Chance);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            var m = new Move()
            {
                Name = "Cross Poison",
                MovementFlags = (MoveMovementFlags)0,
                Type = TypeId.Poison,
                Power = 38u,
                Accuracy = 100u,
                Range = MoveRangeId.Cross,
                Effect0 = MoveEffectId.ChanceToPoisonTarget,
                Effect0Chance = 10u,
                Effect1 = MoveEffectId.HighCriticalHitChance,
                Effect1Chance = 0u,
                Effect2 = MoveEffectId.ChanceToPoisonTarget,
                Effect2Chance = 10u,
                Effect3 = MoveEffectId.HighCriticalHitChance,
                Effect3Chance = 0u,
            };

            Assert.Equal("Cross Poison", m.Name);
            Assert.Equal((MoveMovementFlags)0, m.MovementFlags);
            Assert.Equal(TypeId.Poison, m.Type);
            Assert.Equal(38u, m.Power);
            Assert.Equal(100u, m.Accuracy);
            Assert.Equal(MoveRangeId.Cross, m.Range);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect0);
            Assert.Equal(10u, m.Effect0Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect1);
            Assert.Equal(0u, m.Effect1Chance);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, m.Effect2);
            Assert.Equal(10u, m.Effect2Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, m.Effect3);
            Assert.Equal(0u, m.Effect3Chance);

            // Add Array equal test when possible
        }
    }
}
