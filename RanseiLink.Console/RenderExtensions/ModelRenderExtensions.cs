#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.Text;

namespace RanseiLink.Console;

public static partial class RenderExtensions
{
    private static string RenderQuantityForEvolutionCondition(EvolutionConditionId id, int quantity)
    {
        switch (id)
        {
            case EvolutionConditionId.Hp:
            case EvolutionConditionId.Attack:
            case EvolutionConditionId.Defence:
            case EvolutionConditionId.Speed:
                return $"({quantity}) ";

            case EvolutionConditionId.Link:
                return $"({quantity}%) ";

            case EvolutionConditionId.Kingdom:
                return $"({(KingdomId)quantity}) ";

            case EvolutionConditionId.WarriorGender:
                return $"({(GenderId)quantity}) ";

            case EvolutionConditionId.Item:
                return $"({(ItemId)quantity}) ";

            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                return "";

            default:
                throw new ArgumentException("Unexpected enum value");
        }
    }

    private static string RenderQuantityForRankUpCondition(RankUpConditionId id, int quantity)
    {
        switch (id)
        {
            case RankUpConditionId.Unknown:
            case RankUpConditionId.NoCondition:
            case RankUpConditionId.Unused_1:
            case RankUpConditionId.Unused_2:
            case RankUpConditionId.Unused_3:
            case RankUpConditionId.Unused_4:
                return $"{quantity}";

            case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
            case RankUpConditionId.AtLeastNGalleryPokemon:
            case RankUpConditionId.AtLeastNGalleryWarriors:
                return $"{quantity}";

            case RankUpConditionId.AfterCompletingEpisode:
            case RankUpConditionId.DuringEpisode:
                return $"{(EpisodeId)quantity}";

            case RankUpConditionId.MonotypeGallery:
                return $"{(TypeId)quantity}";

            case RankUpConditionId.WarriorInSameArmyNotNearby:
            case RankUpConditionId.WarriorInSameKingdom:
                return $"{(WarriorLineId)quantity}";

            default:
                throw new ArgumentException($"Unexpeted {nameof(RankUpConditionId)}");
        }
    }

    /*
    public static void Render(this IConsole console, Pokemon pokemon, PokemonId id)
    {
        console.WriteProperty("Evolution Conditions", string.Format("{0} {1}/ {2} {3}",
            pokemon.EvolutionCondition1,
            RenderQuantityForEvolutionCondition(pokemon.EvolutionCondition1, pokemon.QuantityForEvolutionCondition1),
            pokemon.EvolutionCondition2,
            RenderQuantityForEvolutionCondition(pokemon.EvolutionCondition2, pokemon.QuantityForEvolutionCondition2)
            ));

        foreach (var evo in pokemon.Evolutions)
        {
            console.WriteLine($"      - {evo}");
        }

        var sb1 = new StringBuilder();
        var sb2 = new StringBuilder();
        foreach (var location in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            if (pokemon.GetEncounterable(location, false))
            {
                sb1.Append(location);
                sb1.Append(", ");
            }
            if (pokemon.GetEncounterable(location, true))
            {
                sb2.Append(location);
                sb2.Append(", ");
            }
        }
    }
    */
    private static string RenderQuantityForMoveEffect(MoveEffectId id, int value)
    {
        switch (id)
        {
            case MoveEffectId.Unused_16:
            case MoveEffectId.HotSpring_WaterBucket_RevivalNode:
            case MoveEffectId.Unused_18:
            case MoveEffectId.Unused_19:
            
            case MoveEffectId.Unused_1:
                return $"({value}?) ";

            case MoveEffectId.TargetInvincibleForOneTurn:
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
                return "";

            case MoveEffectId.InflictsFixedHpDamage:
                return $"({value} HP) ";

            case MoveEffectId.ChanceToRaiseUserDefence:
            case MoveEffectId.ChanceToLowerUserDefence:
            case MoveEffectId.ChanceToRaiseUserSpeed:
            case MoveEffectId.ChanceToLowerUserSpeed:
            case MoveEffectId.ChanceToLowerTargetAttack:
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
            case MoveEffectId.HealsUserByPercentageOfDamageDealt:
                return $"({value}%) ";
            default:
                throw new ArgumentException($"Unexpected {nameof(MoveEffectId)} value of {id}");
        }
    }
    
    public static void Render(this IConsole console, MaxLink maxSync, WarriorId id)
    {
        console.WriteTitle($"{id}");
        foreach (var pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            console.WriteProperty(pid.ToString(), maxSync.GetMaxLink(pid).ToString());
        }
    }

    /*
    public static void Render(this IConsole console, ScenarioWarrior scenarioWarrior, ScenarioId scenarioId, int scenarioWarriorId)
    {
        console.WriteProperty("Scenario Pokemon", string.Join(", ", 
            Enumerable.Range(0, 8)
            .Select(i => scenarioWarrior.ScenarioPokemonIsDefault(i) ? "<default>" : scenarioWarrior.GetScenarioPokemon(i).ToString()))
            );
    }*/

    public static void Render(this IConsole console, WarriorNameTable model)
    {
        console.WriteTitle($"Warrior Name Table");
        for (int i = 0; i < WarriorNameTable.EntryCount; i++)
        {
            console.WriteProperty(i.ToString().PadLeft(3, '0'), model.GetEntry(i));
        }
    }

    public static void Render(this IConsole console, ScenarioAppearPokemon model, ScenarioId id)
    {
        console.WriteTitle($"{id}");
        foreach (var pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            if (model.GetCanAppear(pid))
            {
                console.WriteLine($"    {pid}");
            }
        }
    }

    public static void Render(this IConsole console, ScenarioKingdom model, ScenarioId id)
    {
        console.WriteTitle($"{id} (Army Assignment)");
        foreach (var k in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            console.WriteProperty(k, model.GetArmy(k));
        }
    }

    public static void Render(this IConsole console, ScenarioBuilding model, ScenarioId id)
    {
        console.WriteTitle($"{id} (Scenario Building)");
        foreach (var k in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            console.WriteTitle(k.ToString());
            for (int i = 0; i < ScenarioBuilding.SlotCount; i++)
            {
                console.WriteProperty(i, $"InitExp: {model.GetInitialExp(k, i)}, Unknown2: {model.GetUnknown2(k, i)}");
            }
        }
    }
}
