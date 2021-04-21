using Core.Enums;
using Core.Models;
using Xunit;

namespace Tests.ModelTests
{
    public class AbilityTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            var lastBastion = new Ability(new byte[]
            {
                0x4C, 0x61, 0x73, 0x74, 
                0x20, 0x42, 0x61, 0x73, 
                0x74, 0x69, 0x6F, 0x6E, 
                0x00, 0x00, 0x00, 0x02, 
                0x41, 0x08, 0x00, 0x00
            });

            Assert.Equal("Last Bastion", lastBastion.Name);
            Assert.Equal(2, lastBastion.Effect1Amount);
            Assert.Equal(AbilityEffectId.IncreaseUserAttack, lastBastion.Effect1);
            Assert.Equal(AbilityEffectId.IncreaseUserDefence, lastBastion.Effect2);
            Assert.Equal(2, lastBastion.Effect2Amount);
        }
    }
}
