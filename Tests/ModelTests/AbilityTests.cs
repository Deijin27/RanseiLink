using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Xunit;

namespace Tests.ModelTests
{
    public class AbilityTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IAbility a = new Ability(new byte[]
            {
                0x4C, 0x61, 0x73, 0x74, 
                0x20, 0x42, 0x61, 0x73, 
                0x74, 0x69, 0x6F, 0x6E, 
                0x00, 0x00, 0x00, 0x02, 
                0x41, 0x08, 0x00, 0x00
            });

            Assert.Equal("Last Bastion", a.Name);
            Assert.Equal(2u, a.Effect1Amount);
            Assert.Equal(AbilityEffectId.IncreaseUserAttack, a.Effect1);
            Assert.Equal(AbilityEffectId.IncreaseUserDefence, a.Effect2);
            Assert.Equal(2u, a.Effect2Amount);
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

            IAbility a = new Ability
            {
                Name = "Last Bastion",
                Effect1Amount = 2u,
                Effect1 = AbilityEffectId.IncreaseUserAttack,
                Effect2 = AbilityEffectId.IncreaseUserDefence,
                Effect2Amount = 2u
            };

            Assert.Equal("Last Bastion", a.Name);
            Assert.Equal(2u, a.Effect1Amount);
            Assert.Equal(AbilityEffectId.IncreaseUserAttack, a.Effect1);
            Assert.Equal(AbilityEffectId.IncreaseUserDefence, a.Effect2);
            Assert.Equal(2u, a.Effect2Amount);

            Assert.Equal(lastBastionData, a.Data);
        }
    }
}
