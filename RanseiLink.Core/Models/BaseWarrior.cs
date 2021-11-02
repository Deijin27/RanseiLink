using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models
{
    public class BaseWarrior : BaseDataWindow, IBaseWarrior
    {
        public const int DataLength = 0x14;
        public BaseWarrior(byte[] data) : base(data, DataLength) { }
        public BaseWarrior() : this(new byte[DataLength]) { }

        public WarriorSpriteId Sprite
        {
            get => (WarriorSpriteId)GetUInt32(0, 8, 0);
            set => SetUInt32(0, 8, 0, (uint)value);
        }

        public WarriorSprite2Id Sprite_Unknown
        {
            get => (WarriorSprite2Id)GetUInt32(0, 8, 8);
            set => SetUInt32(0, 8, 8, (uint)value);
        }

        public GenderId Gender
        {
            get => (GenderId)GetUInt32(0, 1, 16);
            set => SetUInt32(0, 1, 16, (uint)value);
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

        public TypeId Weakness1
        {
            get => (TypeId)GetUInt32(1, 5, 10);
            set => SetUInt32(1, 5, 10, (uint)value);
        }

        public TypeId Weakness2
        {
            get => (TypeId)GetUInt32(1, 5, 15);
            set => SetUInt32(1, 5, 15, (uint)value);
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

        public RankUpConditionId RankUpCondition1
        {
            get => (RankUpConditionId)GetUInt32(2, 4, 24);
            set => SetUInt32(2, 4, 24, (uint)value);
        }

        public RankUpConditionId RankUpCondition2
        {
            get => (RankUpConditionId)GetUInt32(2, 4, 28);
            set => SetUInt32(2, 4, 28, (uint)value);
        }

        public uint Quantity1ForRankUpCondition
        {
            get => GetUInt32(4, 9, 9);
            set => SetUInt32(4, 9, 9, value);
        }

        public uint Quantity2ForRankUpCondition
        {
            get => GetUInt32(4, 9, 18);
            set => SetUInt32(4, 9, 18, value);
        }

        public WarriorId RankUp
        {
            get => (WarriorId)GetUInt32(2, 9, 15);
            set => SetUInt32(2, 9, 15, (uint)value);
        }

        public PokemonId RankUpPokemon1
        {
            get => (PokemonId)GetUInt32(1, 9, 20);
            set => SetUInt32(1, 9, 20, (uint)value);
        }

        public PokemonId RankUpPokemon2
        {
            get => (PokemonId)GetUInt32(4, 9, 0);
            set => SetUInt32(4, 9, 0, (uint)value);
        }

        public uint RankUpLink
        {
            get => GetUInt32(0, 7, 25);
            set => SetUInt32(0, 7, 25, value);
        }

        public IBaseWarrior Clone()
        {
            return new BaseWarrior((byte[])Data.Clone());
        }
    }
}
