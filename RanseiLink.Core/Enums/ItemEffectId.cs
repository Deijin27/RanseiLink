
namespace RanseiLink.Core.Enums
{
    public enum ItemEffectId
    {
        /// <summary>
        /// Unknown
        /// </summary>
        NoEffect,

        /// <summary>
        /// Slightly increases Warriors Powers
        /// </summary>
        SmallIncreaseWarriorPower,

        /// <summary>
        /// Increases Warriors Powers
        /// </summary>
        MediumIncreaseWarriorPower,

        /// <summary>
        /// Greatly increases Warriors Powers
        /// </summary>
        LargeIncreaseWarriorPower,

        /// <summary>
        /// Heals Pokémon gradually
        /// </summary>
        HealPokemonEachTurn,

        /// <summary>
        /// Increases Attack but may prevent Pokémon from moving	
        /// </summary>
        IncreaseAttackMayPreventMove,

        /// <summary>
        /// Increases Defence but may prevent Pokémon from moving
        /// </summary>
        IncreaseDefenceMayPreventMove,

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
        /// Heals Hp
        /// </summary>
        HealsHp,

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
        /// Increases the Link percentage gained
        /// </summary>
        IncreasePercentageLinkGained,

        /// <summary>
        /// Makes it easier to Link with a Pokémon
        /// </summary>
        EasierToLinkWithPokemon,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown_16,

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
}