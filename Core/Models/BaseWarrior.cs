using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class BaseWarrior : BaseDataWindow, IBaseWarrior
    {
        public const int DataLength = 0x14;
        public BaseWarrior(byte[] data) : base(data, DataLength) { }
        public BaseWarrior() : base(new byte[DataLength], DataLength) { }

        public WarriorSpriteId Sprite
        {
            get => (WarriorSpriteId)GetUInt32(0, 8, 0);
            set => SetUInt32(0, 8, 0, (uint)value);
        }

        public uint WarriorName
        {
            get => GetUInt32(0, 8, 17);
            set => SetUInt32(0, 8, 17, value);
        }

        public TypeId Speciality1
        {
            get => (TypeId)GetUInt32(1, 5, 0);
            set => SetUInt32(1, 5, 0, (uint)value);
        }

        public TypeId Speciality2
        {
            get => (TypeId)GetUInt32(1, 5, 5);
            set => SetUInt32(1, 5, 5, (uint)value);
        }

        public WarriorSkillId Skill
        {
            get => (WarriorSkillId)GetUInt32(2, 7, 0);
            set => SetUInt32(2, 7, 0, (uint)value);
        }

        public uint Power
        {
            get => GetUInt32(3, 7, 0);
            set => SetUInt32(3, 7, 0, value);
        }

        public uint Wisdom
        {
            get => GetUInt32(3, 7, 7);
            set => SetUInt32(3, 7, 7, value);
        }

        public uint Charisma
        {
            get => GetUInt32(3, 7, 14);
            set => SetUInt32(3, 7, 14, value);
        }

        public uint Capacity
        {
            get => GetUInt32(3, 4, 21);
            set => SetUInt32(3, 4, 21, value);
        }

        public WarriorId Evolution
        {
            get => (WarriorId)GetUInt32(2, 8, 15);
            set => SetUInt32(2, 8, 15, (uint)value);
        }

        public IBaseWarrior Clone()
        {
            return new BaseWarrior((byte[])Data.Clone());
        }
    }
}
