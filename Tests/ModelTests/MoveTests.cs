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
            var crossPoison = new Move(new byte[] 
            { 
                0x43, 0x72, 0x6F, 0x73, 0x73, 0x20, 0x50, 0x6F, 0x69, 0x73, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 
                0xC7, 0xE4, 0xA4, 0x50, 0xFF, 0xFE, 0x5D, 0x01, 0x09, 0xC0, 0x49, 0x81, 0x09, 0x00, 0x24, 0xF3, 
                0xA9, 0x42, 0x04, 0x00 
            });

            Assert.Equal("Cross Poison", crossPoison.Name);
            Assert.Equal((MoveMovementFlags)0, crossPoison.MovementFlags);
            Assert.Equal(TypeId.Poison, crossPoison.Type);
            Assert.Equal(38, crossPoison.Power);
            Assert.Equal(100, crossPoison.Accuracy);
            Assert.Equal(MoveRangeId.Cross, crossPoison.Range);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, crossPoison.Effect0);
            Assert.Equal(10, crossPoison.Effect0Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, crossPoison.Effect1);
            Assert.Equal(0, crossPoison.Effect1Chance);
            Assert.Equal(MoveEffectId.ChanceToPoisonTarget, crossPoison.Effect2);
            Assert.Equal(10, crossPoison.Effect2Chance);
            Assert.Equal(MoveEffectId.HighCriticalHitChance, crossPoison.Effect3);
            Assert.Equal(0, crossPoison.Effect3Chance);
        }
    }
}
