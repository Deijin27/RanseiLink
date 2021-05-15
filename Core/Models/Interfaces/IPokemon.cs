using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IPokemon : IDataWrapper
    {
        AbilityId Ability1 { get; set; }
        AbilityId Ability2 { get; set; }
        AbilityId Ability3 { get; set; }
        uint Atk { get; set; }
        uint Def { get; set; }
        EvolutionConditionId EvolutionCondition1 { get; set; }
        EvolutionConditionId EvolutionCondition2 { get; set; }
        uint Hp { get; set; }
        bool IsLegendary { get; set; }
        MoveId Move { get; set; }
        string Name { get; set; }
        uint NameOrderIndex { get; set; }
        uint QuantityForEvolutionCondition1 { get; set; }
        uint QuantityForEvolutionCondition2 { get; set; }
        uint Spe { get; set; }
        TypeId Type1 { get; set; }
        TypeId Type2 { get; set; }

        bool GetEncounterable(LocationId location, bool requiresLevel2);
        void SetEncounterable(LocationId location, bool requiresLevel2, bool value);
    }
}
