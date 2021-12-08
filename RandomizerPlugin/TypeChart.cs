
using RanseiLink.Core.Enums;

namespace RandomizerPlugin;

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
}
