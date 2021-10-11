using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IBaseWarrior : IDataWrapper, ICloneable<IBaseWarrior>
    {
        uint WarriorName { get; set; }
        TypeId Speciality1 { get; set; }
        TypeId Speciality2 { get; set; }
        uint Power { get; set; }
        uint Wisdom { get; set; }
        uint Charisma { get; set; }
        uint Capacity { get; set; }
        WarriorId Evolution { get; set; }
        WarriorSpriteId Sprite { get; set; }
        WarriorSkillId Skill { get; set; }
    }
}