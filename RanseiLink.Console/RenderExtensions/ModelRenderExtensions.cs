using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using System;
using System.Text;

namespace RanseiLink.Console;

public static partial class RenderExtensions
{
    private static string RenderQuantityForEvolutionCondition(EvolutionConditionId id, uint quantity)
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
                return $"({(KingdomId)(int)quantity}) ";

            case EvolutionConditionId.WarriorGender:
                return $"({(GenderId)(int)quantity}) ";

            case EvolutionConditionId.Item:
                return $"({(ItemId)(int)quantity}) ";

            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                return "";

            default:
                throw new ArgumentException("Unexpected enum value");
        }
    }

    private static string RenderQuantityForRankUpCondition(RankUpConditionId id, uint quantity)
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

    public static void Render(this IConsole console, IPokemon pokemon, PokemonId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", pokemon.Name);
        console.WriteProperty("Types", $"{pokemon.Type1} / {pokemon.Type2}");
        console.WriteProperty("Abilities", $"{pokemon.Ability1} / {pokemon.Ability2} / {pokemon.Ability3}");
        console.WriteProperty("Move", pokemon.Move.ToString());

        console.WriteProperty("Evolution Conditions", string.Format("{0} {1}/ {2} {3}",
            pokemon.EvolutionCondition1,
            RenderQuantityForEvolutionCondition(pokemon.EvolutionCondition1, pokemon.QuantityForEvolutionCondition1),
            pokemon.EvolutionCondition2,
            RenderQuantityForEvolutionCondition(pokemon.EvolutionCondition2, pokemon.QuantityForEvolutionCondition2)
            ));

        var evolutionRange = pokemon.EvolutionRange;
        console.WriteProperty("Evolution Table Range",
            evolutionRange.CanEvolve
                ? $"{evolutionRange.MinEntry} - {evolutionRange.MaxEntry}"
                : "does not evolve");

        console.WriteProperty("Stats", $"{pokemon.Hp} HP / {pokemon.Atk} Atk / {pokemon.Def} Def / {pokemon.Spe} Spe");
        console.WriteProperty("Movement Range", pokemon.MovementRange.ToString());
        console.WriteProperty("Is Legendary", pokemon.IsLegendary.ToString());
        console.WriteProperty("NatDex Number", pokemon.NationalPokedexNumber.ToString());
        console.WriteProperty("Name Alphabetical Sort Index", pokemon.NameOrderIndex.ToString());

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

        console.WriteProperty("Default Encounterable", sb1.ToString().TrimEnd(',', ' '));
        console.WriteProperty("Lv2 Encounterable", sb2.ToString().TrimEnd(',', ' '));
    }

    private static string RenderQuantityForMoveEffect(MoveEffectId id, uint value)
    {
        switch (id)
        {
            case MoveEffectId.Unused_16:
            case MoveEffectId.HotSpring_WaterBucket_RevivalNode:
            case MoveEffectId.Unused_18:
            case MoveEffectId.Unused_19:
            case MoveEffectId.Multihit_Unused:
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

    public static void Render(this IConsole console, IMove move, MoveId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", move.Name);
        console.WriteProperty("Type", move.Type.ToString());
        console.WriteProperty("Power", move.Power.ToString());
        console.WriteProperty("Accuracy", $"{move.Accuracy}%");
        console.WriteProperty("MovementFlags", move.MovementFlags.ToString());
        console.WriteProperty("Range", move.Range.ToString());
        console.WriteProperty("Effects", $"{move.Effect1} {RenderQuantityForMoveEffect(move.Effect1, move.Effect1Chance)}/ {move.Effect2} {RenderQuantityForMoveEffect(move.Effect2, move.Effect2Chance)}");
        console.WriteProperty("Unused Effect Duplicates", $"{move.Effect3} {RenderQuantityForMoveEffect(move.Effect3, move.Effect3Chance)}/ {move.Effect4} {RenderQuantityForMoveEffect(move.Effect4, move.Effect4Chance)}");
    }

    public static void Render(this IConsole console, IAbility ability, AbilityId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", ability.Name);
        console.WriteProperty("Effect1", $"{ability.Effect1} ({ability.Effect1Amount})");
        console.WriteProperty("Effect2", $"{ability.Effect2} ({ability.Effect2Amount})");
    }

    public static void Render(this IConsole console, IWarriorSkill saihai, WarriorSkillId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", saihai.Name);
        console.WriteProperty("Effect1", $"{saihai.Effect1} ({saihai.Effect1Amount})");
        console.WriteProperty("Effect2", $"{saihai.Effect2} ({saihai.Effect2Amount})");
        console.WriteProperty("Effect3", $"{saihai.Effect3} ({saihai.Effect3Amount})");
        console.WriteProperty("Target", saihai.Target.ToString());
        console.WriteProperty("Duration", saihai.Duration.ToString());
    }

    public static void Render(this IConsole console, IGimmick gimmick, GimmickId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", gimmick.Name);
        console.WriteProperty("Attack Type", gimmick.AttackType);
        console.WriteProperty("Destroy Type", gimmick.DestroyType);
        console.WriteProperty("Animation 1", gimmick.Animation1);
        console.WriteProperty("Animation 2", gimmick.Animation2);
        console.WriteProperty("Range", gimmick.Range);
        console.WriteProperty("Image", gimmick.Image);
        console.WriteProperty("State-1 Sprite", gimmick.State1Sprite);
        console.WriteProperty("State-2 Sprite", gimmick.State2Sprite);
        console.WriteProperty("Effect", gimmick.Effect);
    }

    public static void Render(this IConsole console, IBuilding building, BuildingId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", building.Name);
        console.WriteProperty("Kingdom", building.Kingdom.ToString());
    }

    public static void Render(this IConsole console, IItem item, ItemId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", item.Name);
        console.WriteProperty("Shop Price Multiplier", item.ShopPriceMultiplier);
    }

    public static void Render(this IConsole console, IKingdom kingdom, KingdomId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", kingdom.Name);
        console.WriteProperty("Battle Config", kingdom.BattleConfig);
        console.WriteProperty("Can Battle", "\n    - " + string.Join("\n    - ", kingdom.MapConnections));
    }

    public static void Render(this IConsole console, IMaxLink maxSync, WarriorId id)
    {
        console.WriteTitle($"{id}");
        foreach (var pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            console.WriteProperty(pid.ToString(), maxSync.GetMaxLink(pid).ToString());
        }
    }

    public static void Render(this IConsole console, IScenarioPokemon scenarioPokemon, ScenarioId scenarioId, int scenarioPokemonId)
    {
        console.WriteTitle($"Scenario = {scenarioId}, Entry = {scenarioPokemonId}");
        console.WriteProperty("Pokemon", scenarioPokemon.Pokemon.ToString());
        console.WriteProperty("Ability", scenarioPokemon.Ability.ToString());
        console.WriteProperty("IVs", $"Hp {scenarioPokemon.HpIv} / Atk {scenarioPokemon.AtkIv} / Def {scenarioPokemon.DefIv} / Spe {scenarioPokemon.SpeIv}");
    }

    public static void Render(this IConsole console, IScenarioWarrior scenarioWarrior, ScenarioId scenarioId, int scenarioWarriorId)
    {
        console.WriteTitle($"Scenario = {scenarioId}, Entry = {scenarioWarriorId}");
        console.WriteProperty("Warrior", scenarioWarrior.Warrior);
        console.WriteProperty("Class", scenarioWarrior.Class);
        console.WriteProperty("Army", scenarioWarrior.Army);
        console.WriteProperty("Kingdom", scenarioWarrior.Kingdom);
        console.WriteProperty("Scenario Pokemon Entry", scenarioWarrior.ScenarioPokemonIsDefault ? "<default>" : scenarioWarrior.ScenarioPokemon.ToString());
    }

    public static void Render(this IConsole console, IEvolutionTable model)
    {
        console.WriteTitle($"Evolution Table");
        for (int i = 0; i < EvolutionTable.DataLength; i++)
        {
            console.WriteProperty(i.ToString().PadLeft(3, '0'), model.GetEntry(i));
        }
    }
    public static void Render(this IConsole console, IWarriorNameTable model)
    {
        console.WriteTitle($"Warrior Name Table");
        for (int i = 0; i < WarriorNameTable.EntryCount; i++)
        {
            console.WriteProperty(i.ToString().PadLeft(3, '0'), model.GetEntry((uint)i));
        }
    }

    public static void Render(this IConsole console, IBaseWarrior model, WarriorId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Sprite", model.Sprite);
        console.WriteProperty("Warrior Name Table Entry", model.WarriorName);
        console.WriteProperty("Specialities", $"{model.Speciality1} / {model.Speciality2}");
        console.WriteProperty("Weaknesses", $"{model.Weakness1} / {model.Weakness2}");
        console.WriteProperty("Skill", model.Skill);
        console.WriteProperty("Stats", $"Power {model.Power} / Wisdom {model.Wisdom} / Charisma {model.Charisma}");
        console.WriteProperty("Capacity", model.Capacity);
        console.WriteProperty("Gender", model.Gender);
        console.WriteProperty("Rank Up Into", model.RankUp);
        console.WriteProperty("Rank Up Pokemon", $"{model.RankUpPokemon1} / {model.RankUpPokemon2}");
        console.WriteProperty("Rank Up Link", $"{model.RankUpLink}%");
        console.WriteProperty("Rank Up Condition 1", model.RankUpCondition1);
        console.WriteProperty("Rank Up Condition 2",
            $"{model.RankUpCondition2} ({RenderQuantityForRankUpCondition(model.RankUpCondition2, model.Quantity1ForRankUpCondition)}, {RenderQuantityForRankUpCondition(model.RankUpCondition2, model.Quantity2ForRankUpCondition)})");
    }

    public static void Render(this IConsole console, IScenarioAppearPokemon model, ScenarioId id)
    {
        console.WriteTitle($"{id}");
        foreach (var pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            if (model.GetCanAppear(pid))
            {
                console.Output.WriteLine($"    {pid}");
            }
        }
    }

    public static void Render(this IConsole console, IEventSpeaker model, EventSpeakerId id)
    {
        console.WriteTitle($"{id}");
        console.WriteProperty("Name", model.Name);
        console.WriteProperty("Sprite", model.Sprite);
    }

    public static void Render(this IConsole console, IScenarioKingdom model, ScenarioId id)
    {
        console.WriteTitle($"{id} (Army Assignment)");
        foreach (var k in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            console.WriteProperty(k, model.GetArmy(k));
        }
    }
}
