using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IPokemon : IDataWrapper, ICloneable<IPokemon>
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
    uint NationalPokedexNumber { get; set; }
    uint MovementRange { get; set; }
    PokemonEvolutionRange EvolutionRange { get; set; }
    uint UnknownValue { get; set; }

    bool GetEncounterable(KingdomId kingdom, bool requiresLevel2);
    void SetEncounterable(KingdomId kingdom, bool requiresLevel2, bool value);
}
