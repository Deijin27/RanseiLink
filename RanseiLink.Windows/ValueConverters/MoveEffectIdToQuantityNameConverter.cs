using RanseiLink.Core.Enums;
using System;

namespace RanseiLink.Windows.ValueConverters;

public class MoveEffectIdToQuantityNameConverter : ValueConverter<MoveEffectId, string>
{
    protected override string Convert(MoveEffectId value)
    {
        switch (value)
        {
            case MoveEffectId.Unused_16:
            case MoveEffectId.HotSpring_WaterBucket_RevivalNode:
            case MoveEffectId.Unused_18:
            case MoveEffectId.Unused_1:
            case MoveEffectId.Unused_19:
                return $"Unused Quantity";

            case MoveEffectId.ThawsUserIfFrozen:
            case MoveEffectId.WakesTargetIfSleep:
            case MoveEffectId.DestroysTargetsConsumableItem:
            case MoveEffectId.DoublePowerIfTargetUnderHalfHp:
            case MoveEffectId.DoublePowerIfTargetPoisoned:
            case MoveEffectId.FailsIfTargetNotAsleep:
            case MoveEffectId.UsesTargetsConsumableItem:
            case MoveEffectId.NeverMisses:
            case MoveEffectId.DamagesUserIfMisses:
            case MoveEffectId.UserTeleportsRandomly:
            case MoveEffectId.CannotBeUsedTurnAfterHitting:
            case MoveEffectId.UserHasZeroRangeForOneTurn:
            case MoveEffectId.LowerUserRangeAndDefenceForOneTurn:
            case MoveEffectId.Hits2Times:
            case MoveEffectId.Hits2To3Times:
            case MoveEffectId.Hits2To5Times:
            case MoveEffectId.Hits4To5Times:
            case MoveEffectId.Hits5Times:
            case MoveEffectId.VanishesAndHitsStartOfNextTurn:
            case MoveEffectId.VanishesWithTargetAndHitsStartOfNextTurn:
            case MoveEffectId.HitsStartOfTurnAfterNext:
            case MoveEffectId.SwitchWithTargetIfItsAtDestination:
            case MoveEffectId.SwitchTargetWithPokemonBehindIt:
            case MoveEffectId.HighCriticalHitChance:
            case MoveEffectId.IgnoreTargetStatModifiers:
            case MoveEffectId.StrongerTheFasterUserIsThanTarget:
            case MoveEffectId.StrongerTheSlowerUserIsThanTarget:
            case MoveEffectId.DoublePowerIfTargetDamagedThisTurn:
            case MoveEffectId.UsesTargetsAttackStat:
            case MoveEffectId.DoublePowerIfTargetSleep:
            case MoveEffectId.DoublePowerIfTargetStatused:
            case MoveEffectId.DoublePowerWithConsecutiveUses:
            case MoveEffectId.NoEffect:
            case MoveEffectId.TargetInvincibleForOneTurn:
                return "N/A";

            case MoveEffectId.InflictsFixedHpDamage:
                return "HP Dealt";

            case MoveEffectId.ChanceToLowerUserAttackAndDefence:
            case MoveEffectId.ChanceToLowerTargetRange:
            case MoveEffectId.ChanceToLowerTargetSpeed:
            case MoveEffectId.ChanceLowerTargetAccuracy:
            case MoveEffectId.ChanceToParalyzeTarget:
            case MoveEffectId.ChanceToSleepTarget:
            case MoveEffectId.ChanceToPoisonTarget:
            case MoveEffectId.ChanceToBadlyPoisonTarget:
            case MoveEffectId.ChanceToBurnTarget:
            case MoveEffectId.ChanceToFreezeTarget:
            case MoveEffectId.ChanceToConfuseTarget:
            case MoveEffectId.ChanceToFlinchTarget:
            case MoveEffectId.ChanceToRaiseUserAttack:
            case MoveEffectId.ChanceToLowerUserAttack:
            case MoveEffectId.ChanceToLowerTargetDefence:
            case MoveEffectId.ChanceToLowerTargetAttack:
            case MoveEffectId.ChanceToRaiseUserDefence:
            case MoveEffectId.ChanceToLowerUserDefence:
            case MoveEffectId.ChanceToRaiseUserSpeed:
            case MoveEffectId.ChanceToLowerUserSpeed:
                return "Percentage Chance";

            case MoveEffectId.HealsUserByPercentageOfDamageDealt:
                return "Percentage Healed";
            default:
                throw new ArgumentException($"Unexpected {nameof(MoveEffectId)} value of {value}");
        }
    }

    protected override MoveEffectId ConvertBack(string value)
    {
        throw new NotImplementedException();
    }
}
