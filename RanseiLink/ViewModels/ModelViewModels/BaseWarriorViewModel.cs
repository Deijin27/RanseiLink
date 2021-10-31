using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels
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
        public WarriorSprite2Id[] Sprite2Items { get; } = EnumUtil.GetValues<WarriorSprite2Id>().ToArray();

        public WarriorSprite2Id Sprite_Unknown
        {
            get => Model.Sprite_Unknown;
            set => RaiseAndSetIfChanged(Model.Sprite_Unknown, value, v => Model.Sprite_Unknown = v);
        }

        public GenderId[] GenderItems { get; } = EnumUtil.GetValues<GenderId>().ToArray();

        public GenderId Gender
        {
            get => Model.Gender;
            set => RaiseAndSetIfChanged(Model.Gender, value, v => Model.Gender = v);
        }

        public uint WarriorName
        {
            get => Model.WarriorName;
            set => RaiseAndSetIfChanged(Model.WarriorName, value, v => Model.WarriorName = v);
        }

        public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();

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

        public TypeId Weakness1
        {
            get => Model.Weakness1;
            set => RaiseAndSetIfChanged(Model.Weakness1, value, v => Model.Weakness1 = v);
        }

        public TypeId Weakness2
        {
            get => Model.Weakness2;
            set => RaiseAndSetIfChanged(Model.Weakness2, value, v => Model.Weakness2 = v);
        }

        public WarriorSkillId[] WarriorSkillItems { get; } = EnumUtil.GetValues<WarriorSkillId>().ToArray();

        public WarriorSkillId Skill
        {
            get => Model.Skill;
            set => RaiseAndSetIfChanged(Model.Skill, value, v => Model.Skill = v);
        }

        public WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();

        public WarriorId RankUp
        {
            get => Model.RankUp;
            set => RaiseAndSetIfChanged(Model.RankUp, value, v => Model.RankUp = v);
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

        public PokemonId[] PokemonItems { get; } = EnumUtil.GetValues<PokemonId>().ToArray();

        public PokemonId RankUpPokemon1
        {
            get => Model.RankUpPokemon1;
            set => RaiseAndSetIfChanged(Model.RankUpPokemon1, value, v => Model.RankUpPokemon1 = value);
        }

        public PokemonId RankUpPokemon2
        {
            get => Model.RankUpPokemon2;
            set => RaiseAndSetIfChanged(Model.RankUpPokemon2, value, v => Model.RankUpPokemon2 = value);
        }

        public uint RankUpLink
        {
            get => Model.RankUpLink;
            set => RaiseAndSetIfChanged(Model.RankUpLink, value, v => Model.RankUpLink = value);
        }

        public RankUpConditionId[] RankUpConditionItems { get; } = EnumUtil.GetValues<RankUpConditionId>().ToArray();

        public RankUpConditionId RankUpCondition1
        {
            get => Model.RankUpCondition1;
            set => RaiseAndSetIfChanged(Model.RankUpCondition1, value, v => Model.RankUpCondition1 = value);
        }

        public RankUpConditionId RankUpCondition2
        {
            get => Model.RankUpCondition2;
            set => RaiseAndSetIfChanged(Model.RankUpCondition2, value, v => Model.RankUpCondition2 = value);
        }

        public uint Quantity1ForRankUpCondition
        {
            get => Model.Quantity1ForRankUpCondition; 
            set => RaiseAndSetIfChanged(Model.Quantity1ForRankUpCondition, value, v => Model.Quantity1ForRankUpCondition = value);
        }

        public uint Quantity2ForRankUpCondition
        {
            get => Model.Quantity2ForRankUpCondition;
            set => RaiseAndSetIfChanged(Model.Quantity2ForRankUpCondition, value, v => Model.Quantity2ForRankUpCondition = value);
        }
    }
}
