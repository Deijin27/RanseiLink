using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IWarriorSkill : IDataWrapper, ICloneable<IWarriorSkill>
    {
        uint Duration { get; set; }
        WarriorSkillEffectId Effect1 { get; set; }
        uint Effect1Amount { get; set; }
        WarriorSkillEffectId Effect2 { get; set; }
        uint Effect2Amount { get; set; }
        WarriorSkillEffectId Effect3 { get; set; }
        uint Effect3Amount { get; set; }
        string Name { get; set; }
        WarriorSkillTargetId Target { get; set; }
    }
}