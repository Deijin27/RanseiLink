using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
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
        WarriorId RankUp { get; set; }
        WarriorSpriteId Sprite { get; set; }
        WarriorSkillId Skill { get; set; }
        WarriorSprite2Id Sprite_Unknown { get; set; }
        GenderId Gender { get; set; }
        PokemonId RankUpPokemon1 { get; set; }
        PokemonId RankUpPokemon2 { get; set; }
        TypeId Weakness2 { get; set; }
        TypeId Weakness1 { get; set; }
        uint RankUpLink { get; set; }
        RankUpConditionId RankUpCondition1 { get; set; }
        RankUpConditionId RankUpCondition2 { get; set; }
        uint Quantity1ForRankUpCondition { get; set; }
        uint Quantity2ForRankUpCondition { get; set; }
    }
}