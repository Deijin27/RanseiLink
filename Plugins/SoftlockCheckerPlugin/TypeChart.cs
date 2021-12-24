using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;

namespace SoftlockCheckerPlugin;

internal static class TypeChart
{
    public static bool IsImmuneTo(this TypeId defendingType, TypeId attackingType)
    {
        switch (defendingType)
        {
            case TypeId.Normal:
                return attackingType == TypeId.Ghost;
            case TypeId.Ground:
                return attackingType == TypeId.Electric;
            case TypeId.Flying:
                return attackingType == TypeId.Ground;
            case TypeId.Ghost:
                return attackingType == TypeId.Normal || attackingType == TypeId.Fighting;
            case TypeId.Dark:
                return attackingType == TypeId.Psychic;
            case TypeId.Steel:
                return attackingType == TypeId.Poison;
            default:
                return false;
        }
    }

    public static bool IsImmuneTo(this IPokemon defendingPokemon, TypeId attackingType)
    {
        return defendingPokemon.Type1.IsImmuneTo(attackingType) || defendingPokemon.Type2.IsImmuneTo(attackingType);
    }

    public static bool Resists(this TypeId defendingType, TypeId attackingType)
    {
        return Resistance[defendingType].Contains(attackingType);
    }

    public static bool IsWeakTo(this TypeId defendingType, TypeId attackingType)
    {
        return Weakness[defendingType].Contains(attackingType);
    }

    public static bool Resists(this IPokemon defendingPokemon, TypeId attackingType)
    {
        if (defendingPokemon.Type1.Resists(attackingType) && !defendingPokemon.Type2.IsWeakTo(attackingType))
        {
            return true;
        }
        if (defendingPokemon.Type2.Resists(attackingType) && !defendingPokemon.Type1.IsWeakTo(attackingType))
        {
            return true;
        }
        return false;
    }

    private static readonly Dictionary<TypeId, HashSet<TypeId>> Resistance = new()
    {
        { TypeId.NoType, new() },
        { TypeId.Normal, new() },
        { TypeId.Fire, new() { TypeId.Fire, TypeId.Grass, TypeId.Ice, TypeId.Bug, TypeId.Steel } },
        { TypeId.Water, new() { TypeId.Fire, TypeId.Water, TypeId.Ice, TypeId.Steel } },
        { TypeId.Electric, new() { TypeId.Electric, TypeId.Flying, TypeId.Steel } },
        { TypeId.Grass, new() { TypeId.Water, TypeId.Electric, TypeId.Grass, TypeId.Ground } },
        { TypeId.Ice, new() { TypeId.Ice } },
        { TypeId.Fighting, new() { TypeId.Bug, TypeId.Rock, TypeId.Dark } },
        { TypeId.Poison, new() { TypeId.Grass, TypeId.Fighting, TypeId.Poison, TypeId.Bug } },
        { TypeId.Ground, new() { TypeId.Poison, TypeId.Rock } },
        { TypeId.Flying, new() { TypeId.Grass, TypeId.Fighting, TypeId.Bug } },
        { TypeId.Psychic, new() { TypeId.Fighting, TypeId.Psychic } },
        { TypeId.Bug, new() { TypeId.Grass, TypeId.Fighting, TypeId.Ground } },
        { TypeId.Rock, new() { TypeId.Normal, TypeId.Fire, TypeId.Poison, TypeId.Flying } },
        { TypeId.Ghost, new() { TypeId.Poison, TypeId.Bug } },
        { TypeId.Dragon, new() { TypeId.Fire, TypeId.Water, TypeId.Electric, TypeId.Grass } },
        { TypeId.Dark, new() { TypeId.Ghost, TypeId.Dark } },
        { TypeId.Steel, new() { TypeId.Normal, TypeId.Grass, TypeId.Ice, TypeId.Flying, TypeId.Psychic, TypeId.Bug, TypeId.Rock, TypeId.Dragon, TypeId.Steel } },
    };

    private static readonly Dictionary<TypeId, HashSet<TypeId>> Weakness = new()
    {
        { TypeId.NoType, new() },
        { TypeId.Normal, new() { TypeId.Fighting } },
        { TypeId.Fire, new() { TypeId.Water, TypeId.Ground, TypeId.Rock } },
        { TypeId.Water, new() { TypeId.Electric, TypeId.Grass } },
        { TypeId.Electric, new() { TypeId.Ground } },
        { TypeId.Grass, new() { TypeId.Fire, TypeId.Ice, TypeId.Poison, TypeId.Flying, TypeId.Bug } },
        { TypeId.Ice, new() { TypeId.Fire, TypeId.Fighting, TypeId.Rock, TypeId.Steel } },
        { TypeId.Fighting, new() { TypeId.Flying, TypeId.Psychic } },
        { TypeId.Poison, new() { TypeId.Ground, TypeId.Psychic } },
        { TypeId.Ground, new() { TypeId.Water, TypeId.Grass, TypeId.Ice } },
        { TypeId.Flying, new() { TypeId.Electric, TypeId.Ice, TypeId.Rock } },
        { TypeId.Psychic, new() { TypeId.Bug, TypeId.Ghost, TypeId.Dark } },
        { TypeId.Bug, new() { TypeId.Fire, TypeId.Flying, TypeId.Rock } },
        { TypeId.Rock, new() { TypeId.Water, TypeId.Grass, TypeId.Fighting, TypeId.Ground, TypeId.Steel } },
        { TypeId.Ghost, new() { TypeId.Ghost, TypeId.Dark } },
        { TypeId.Dragon, new() { TypeId.Ice, TypeId.Dragon } },
        { TypeId.Dark, new() { TypeId.Fighting, TypeId.Bug } },
        { TypeId.Steel, new() { TypeId.Fire, TypeId.Fighting, TypeId.Ground } },
    };
}

