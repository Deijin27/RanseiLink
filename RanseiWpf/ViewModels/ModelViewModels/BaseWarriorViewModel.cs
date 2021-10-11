using Core;
using Core.Enums;
using Core.Models.Interfaces;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class BaseWarriorViewModel : ViewModelBase, IViewModelForModel<IBaseWarrior>
    {
        public IBaseWarrior Model { get; set; }

        public WarriorSpriteId[] SpriteItems { get; } = EnumUtil.GetValues<WarriorSpriteId>().ToArray();

        public WarriorSpriteId Sprite
        {
            get => Model.Sprite;
            set => RaiseAndSetIfChanged(Model.Sprite, value, v => Model.Sprite = v);
        }

        public uint WarriorName
        {
            get => Model.WarriorName;
            set => RaiseAndSetIfChanged(Model.WarriorName, value, v => Model.WarriorName = v);
        }

        public TypeId[] SpecialityItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();

        public TypeId Speciality1
        {
            get => Model.Speciality1;
            set => RaiseAndSetIfChanged(Model.Speciality1, value, v => Model.Speciality1 = v);
        }

        public TypeId Speciality2
        {
            get => Model.Speciality2;
            set => RaiseAndSetIfChanged(Model.Speciality2, value, v => Model.Speciality2 = v);
        }

        public WarriorSkillId[] WarriorSkillItems { get; } = EnumUtil.GetValues<WarriorSkillId>().ToArray();

        public WarriorSkillId Skill
        {
            get => Model.Skill;
            set => RaiseAndSetIfChanged(Model.Skill, value, v => Model.Skill = v);
        }

        public WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();

        public WarriorId Evolution
        {
            get => Model.Evolution;
            set => RaiseAndSetIfChanged(Model.Evolution, value, v => Model.Evolution = v);
        }

        public uint Power
        {
            get => Model.Power;
            set => RaiseAndSetIfChanged(Model.Power, value, v => Model.Power = v);
        }

        public uint Wisdom
        {
            get => Model.Wisdom;
            set => RaiseAndSetIfChanged(Model.Wisdom, value, v => Model.Wisdom = v);
        }

        public uint Charisma
        {
            get => Model.Charisma;
            set => RaiseAndSetIfChanged(Model.Charisma, value, v => Model.Charisma = v);
        }

        public uint Capacity
        {
            get => Model.Capacity;
            set => RaiseAndSetIfChanged(Model.Capacity, value, v => Model.Capacity = v);
        }


    }
}
