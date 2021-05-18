using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Xunit;

namespace Tests.ModelTests
{
    public class WarriorSkillTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IWarriorSkill s = new WarriorSkill(new byte[]
            {
                0x4D, 0x61, 0x79, 0x68,
                0x65, 0x6D, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x01,
                0x85, 0x09, 0x80, 0x66,
                0x28, 0xB8, 0x06, 0x00
            });

            Assert.Equal("Mayhem", s.Name);
            Assert.Equal(1u, s.Effect1Amount);
            Assert.Equal(WarriorSkillEffectId.RaiseRange, s.Effect1);
            Assert.Equal(WarriorSkillEffectId.ClimbHigher, s.Effect2);
            Assert.Equal(0u, s.Effect2Amount);
            Assert.Equal(WarriorSkillEffectId.ChanceToFlinchOpponent, s.Effect3);
            Assert.Equal(40u, s.Effect3Amount);
            Assert.Equal(WarriorSkillTargetId.Self, s.Target);
            Assert.Equal(3u, s.Duration);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IWarriorSkill s = new WarriorSkill()
            {
                Name = "Mayhem",
                Effect1Amount = 1u,
                Effect1 = WarriorSkillEffectId.RaiseRange,
                Effect2 = WarriorSkillEffectId.ClimbHigher,
                Effect2Amount = 0u,
                Effect3 = WarriorSkillEffectId.ChanceToFlinchOpponent,
                Effect3Amount = 40u,
                Target = WarriorSkillTargetId.Self,
                Duration = 3u,
            };

            Assert.Equal("Mayhem", s.Name);
            Assert.Equal(1u, s.Effect1Amount);
            Assert.Equal(WarriorSkillEffectId.RaiseRange, s.Effect1);
            Assert.Equal(WarriorSkillEffectId.ClimbHigher, s.Effect2);
            Assert.Equal(0u, s.Effect2Amount);
            Assert.Equal(WarriorSkillEffectId.ChanceToFlinchOpponent, s.Effect3);
            Assert.Equal(40u, s.Effect3Amount);
            Assert.Equal(WarriorSkillTargetId.Self, s.Target);
            Assert.Equal(3u, s.Duration);

            // Add Array equal test when possible
        }
    }
}
