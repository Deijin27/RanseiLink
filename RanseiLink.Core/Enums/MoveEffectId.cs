
namespace RanseiLink.Core.Enums;

/// <summary>
/// 
/// </summary>
public enum MoveEffectId
{
    /// <summary>
    /// Has no secondary effect
    /// </summary>
    NoEffect = 64,

    /// <summary>
    /// Inflicts fixed HP in damage.
    /// </summary>
    InflictsFixedHpDamage = 0,

    /// <summary>
    /// Has double power if the target is asleep.
    /// </summary>
    DoublePowerIfTargetSleep,

    /// <summary>
    /// Has double power if the target has a major status ailment.
    /// </summary>
    DoublePowerIfTargetStatused,

    /// <summary>
    /// Doubles in power with each consecutive successful use.
    /// </summary>
    DoublePowerWithConsecutiveUses,

    /// <summary>
    /// Ignores the target's stat modifiers.
    /// </summary>
    IgnoreTargetStatModifiers,

    /// <summary>
    /// Power rises the faster the user is compared to the target.
    /// </summary>
    StrongerTheFasterUserIsThanTarget,

    /// <summary>
    /// Power rises the slower the user is compared to the target.
    /// </summary>
    StrongerTheSlowerUserIsThanTarget,

    /// <summary>
    /// Has double power if the target has already taken damage this turn.
    /// </summary>
    DoublePowerIfTargetDamagedThisTurn,

    /// <summary>
    /// Inflicts damage based on the target's Attack stat instead of the user's.
    /// </summary>
    UsesTargetsAttackStat,

    /// <summary>
    /// Has an increased chance for a critical hit.
    /// </summary>
    HighCriticalHitChance,

    /// <summary>
    /// Unknown
    /// </summary>
    Unused_1,

    /// <summary>
    /// Hits 2 times in one turn.
    /// </summary>
    Hits2Times,

    /// <summary>
    /// Is in the unused effect3 for Hits2To5Times and Hits4To5Times moves
    /// </summary>
    Multihit_Unused,

    /// <summary>
    /// Hits 2 to 3 times in one turn. User has 0 range on its next turn.
    /// </summary>
    Hits2To3Times,

    /// <summary>
    /// Hits 2 to 5 times in one turn.
    /// </summary>
    Hits2To5Times,

    /// <summary>
    /// Hits 4 to 5 times in one turn.
    /// </summary>
    Hits4To5Times,

    /// <summary>
    /// Vanishes and hits at the beginning of the next turn.
    /// </summary>
    VanishesAndHitsStartOfNextTurn,

    /// <summary>
    /// Vanishes with target and hits at the beginning of the next turn.
    /// </summary>
    VanishesWithTargetAndHitsStartOfNextTurn,

    /// <summary>
    /// Hits each target at the beginning of the turn after next.
    /// </summary>
    HitsStartOfTurnAfterNext,

    /// <summary>
    /// User switches places with any Pokémon already at its destination.
    /// </summary>
    SwitchWithTargetIfItsAtDestination,

    /// <summary>
    /// Switches each target with the Pokémon behind it.
    /// </summary>
    SwitchTargetWithPokemonBehindIt,

    /// <summary>
    /// Heals the user by a percentage of the damage dealt. 
    /// The percentage number is stored in place of what is normally the effect chance.
    /// </summary>
    HealsUserByPercentageOfDamageDealt,

    /// <summary>
    /// User teleports randomly.
    /// </summary>
    UserTeleportsRandomly,

    /// <summary>
    /// Cannot be used the turn after hitting
    /// </summary>
    CannotBeUsedTurnAfterHitting,

    /// <summary>
    /// User has 0 range on its next turn.
    /// </summary>
    UserHasZeroRangeForOneTurn,

    /// <summary>
    /// Lowers the user's range and Defense until its next turn.
    /// </summary>
    LowerUserRangeAndDefenceForOneTurn,

    /// <summary>
    /// Damages the user if it misses.
    /// </summary>
    DamagesUserIfMisses,

    /// <summary>
    /// Has a chance to lower target's Attack
    /// </summary>
    ChanceToLowerTargetAttack,

    /// <summary>
    /// Has a chance to raise the user's Attack.
    /// </summary>
    ChanceToRaiseUserAttack,

    /// <summary>
    /// Lowers the user's Attack
    /// </summary>
    ChanceToLowerUserAttack,

    /// <summary>
    /// Has a chance to lower target's Defense.
    /// </summary>
    ChanceToLowerTargetDefence,

    /// <summary>
    /// User defence up
    /// </summary>
    ChanceToRaiseUserDefence,

    /// <summary>
    /// User defence down
    /// </summary>
    ChanceToLowerUserDefence,

    /// <summary>
    /// Has a chance to lower target's Speed.
    /// </summary>
    ChanceToLowerTargetSpeed,

    /// <summary>
    /// User speed up
    /// </summary>
    ChanceToRaiseUserSpeed,

    /// <summary>
    /// User speed down
    /// </summary>
    ChanceToLowerUserSpeed,

    /// <summary>
    /// Has a chance to lower target's accuracy.
    /// </summary>
    ChanceLowerTargetAccuracy,

    /// <summary>
    /// Has a chance to paralyze the target.
    /// </summary>
    ChanceToParalyzeTarget,

    /// <summary>
    /// Has a chance to put the target to sleep.
    /// </summary>
    ChanceToSleepTarget,

    /// <summary>
    /// Has a chance to poison the target.
    /// </summary>
    ChanceToPoisonTarget,

    /// <summary>
    /// Has a chance to badly poison the target.
    /// </summary>
    ChanceToBadlyPoisonTarget,

    /// <summary>
    /// Has a chance to burn the target.
    /// </summary>
    ChanceToBurnTarget,

    /// <summary>
    /// Has a chance to freeze the target.
    /// </summary>
    ChanceToFreezeTarget,

    /// <summary>
    /// Has a chance to confuse the target.
    /// </summary>
    ChanceToConfuseTarget,

    /// <summary>
    /// Has a chance to flinch the target.
    /// </summary>
    ChanceToFlinchTarget,

    /// <summary>
    /// Thaws the user out if frozen.
    /// </summary>
    ThawsUserIfFrozen,

    /// <summary>
    /// Wakes up the target if it is sleeping.
    /// </summary>
    WakesTargetIfSleep,

    /// <summary>
    /// Permanently destroys each target's item if it is consumable.
    /// </summary>
    DestroysTargetsConsumableItem,

    /// <summary>
    /// Has double power against Pokémon with less than half their max HP remaining.
    /// </summary>
    DoublePowerIfTargetUnderHalfHp,

    /// <summary>
    /// Has double power against poisoned Pokémon.
    /// </summary>
    DoublePowerIfTargetPoisoned,

    /// <summary>
    /// Only works if the target is asleep.
    /// </summary>
    FailsIfTargetNotAsleep,

    /// <summary>
    /// Uses the target's item if it is consumable.
    /// </summary>
    UsesTargetsConsumableItem,

    /// <summary>
    /// Never Misses.
    /// </summary>
    NeverMisses,

    /// <summary>
    /// Makes target invincible for one turn "Blank is no longer invincible"
    /// </summary>
    TargetInvincibleForOneTurn,

    Unused_16,

    /// <summary>
    /// Used by gimmicks HotSpring, WaterBucket, RevivalNode
    /// May be "Cure all status"
    /// </summary>
    HotSpring_WaterBucket_RevivalNode,

    Unused_18,
    Unused_19,

    /// <summary>
    /// Has a chance to lower target's range by one tile.
    /// </summary>
    ChanceToLowerTargetRange,

    /// <summary>
    /// Has a chance to lower the user's defence and attack.
    /// </summary>
    ChanceToLowerUserAttackAndDefence,
}
