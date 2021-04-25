using Core.Enums;
using Core.Models;
using Xunit;

namespace Tests.ModelTests
{
    public class SaihaiTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            var mayhem = new Saihai(new byte[]
            {
                0x4D, 0x61, 0x79, 0x68, 
                0x65, 0x6D, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x01, 
                0x85, 0x09, 0x80, 0x66, 
                0x28, 0xB8, 0x06, 0x00
            });

            Assert.Equal("Mayhem", mayhem.Name);
            Assert.Equal(1, mayhem.Effect1Amount);
            Assert.Equal(SaihaiEffectId.RaiseRange, mayhem.Effect1);
            Assert.Equal(SaihaiEffectId.ClimbHigher, mayhem.Effect2);
            Assert.Equal(0, mayhem.Effect2Amount);
            Assert.Equal(SaihaiEffectId.ChanceToFlinchOpponent, mayhem.Effect3);
            Assert.Equal(40, mayhem.Effect3Amount);
            Assert.Equal(SaihaiTargetId.Self, mayhem.Target);
            Assert.Equal(3, mayhem.Duration);
        }
    }
}
