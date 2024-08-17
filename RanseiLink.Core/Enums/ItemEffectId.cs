
namespace RanseiLink.Core.Enums;

public enum ItemEffectId
{
    /// <summary>
    /// Unknown
    /// </summary>
    NoEffect,

    /// <summary>
    /// Slightly increases Warriors Power
    /// </summary>
    RaiseWarriorPower,

    /// <summary>
    /// Slightly increases Warriors Wisdom
    /// </summary>
    RaiseWarriorWisdom,

    /// <summary>
    /// Slightly increases Warriors Charisma
    /// </summary>
    RaiseWarriorCharisma,

    /// <summary>
    /// Heals Pokémon gradually
    /// </summary>
    HealPokemonEachTurn,

    /// <summary>
    /// Increases Attack but break
    /// </summary>
    IncreaseAttackMayBreak,

    /// <summary>
    /// Increases Defence but may break
    /// </summary>
    IncreaseDefenceMayBreak,

    /// <summary>
    /// Unknown
    /// </summary>
    Unknown_7,

    /// <summary>
    /// Increases movement range
    /// </summary>
    IncreaseMovementRange,

    /// <summary>
    /// Unknown
    /// </summary>
    Unknown_9,

    /// <summary>
    /// Increase max hp
    /// </summary>
    IncreaseMaxHp,

    /// <summary>
    /// Cures a status condition
    /// </summary>
    CuresStatusCondition,

    /// <summary>
    /// Unknown
    /// </summary>
    Unknown_12,

    /// <summary>
    /// Increases pokémon's energy
    /// </summary>
    IncreasesPokemonEnergy,

    /// <summary>
    /// Increases the Link between warrior and pokemon by a percenta
    /// </summary>
    PercentageMultiplyLinkGained,

    /// <summary>
    /// Makes it easier to Link with a Pokémon
    /// </summary>
    EasierToLinkWithPokemon,

    /// <summary>
    /// Placeholder for complex effects that are probably hardcoded to ids :(
    /// </summary>
    Placeholder,

    /// <summary>
    /// Unknown
    /// </summary>
    Unknown_17,

    /// <summary>
    /// Protects pokemon from a status condition
    /// </summary>
    PreventStatusCondition,

    /// <summary>
    /// Heals HP on specific terrain
    /// </summary>
    HealHpOnTerrain

}