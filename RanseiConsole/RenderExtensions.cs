using CliFx.Infrastructure;
using Core;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Text;

namespace RanseiConsole
{
    static class RenderExtensions
    {
        private static void WithForegroundColor(this IConsole console, ConsoleColor color, Action<IConsole> action)
        {
            var d = console.WithForegroundColor(color);
            action(console);
            d.Dispose();
        }

        private static void WriteTitle(this IConsole console, string title)
        {
            console.WithForegroundColor(ConsoleColor.Magenta, c => c.Output.WriteLine(title));
        }

        private static void WriteProperty(this IConsole console, string propertyName, string propertyValue)
        {
            console.WithForegroundColor(ConsoleColor.White, c => c.Output.Write($"    {propertyName}: "));
            console.WithForegroundColor(ConsoleColor.Gray, c => c.Output.WriteLine(propertyValue));
        }

        public static string RenderQuantityForEvolutionCondition(EvolutionConditionId id, uint quantity)
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

        public static void Render(this IConsole console, IPokemon pokemon, PokemonId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
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

            console.WriteProperty("Stats", $"{pokemon.Hp} HP / {pokemon.Atk} Atk / {pokemon.Def} Def / {pokemon.Spe} Spe");
            console.WriteProperty("Is Legendary", pokemon.IsLegendary.ToString());
            console.WriteProperty("NatDex Number", pokemon.NationalPokedexNumber.ToString());
            console.WriteProperty("Name Alphabetical Sort Index", pokemon.NameOrderIndex.ToString());

            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            foreach (var location in EnumUtil.GetValues<KingdomId>())
            {
                sb1.Append(pokemon.GetEncounterable(location, false));
                sb1.Append(", ");
                sb2.Append(pokemon.GetEncounterable(location, true));
                sb2.Append(", ");
            }
            console.WriteProperty("Default Encounterable:", sb1.ToString());
            console.WriteProperty("Lv2 Encounterable:", sb2.ToString());
        }

        private static string RenderQuantityForMoveEffect(MoveEffectId id, uint value)
        {
            switch (id)
            {
                case MoveEffectId.Unused_15:
                case MoveEffectId.Unused_16:
                case MoveEffectId.Unused_17:
                case MoveEffectId.Unused_18:
                case MoveEffectId.Unused_19:
                case MoveEffectId.Unused_11:
                case MoveEffectId.Unused_12:
                case MoveEffectId.Unused_9:
                case MoveEffectId.Unused_10:
                case MoveEffectId.Unused_7:
                case MoveEffectId.Multihit_Unused:
                case MoveEffectId.Unused_1:
                    return $"({value}?) ";

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
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", move.Name);
            console.WriteProperty("Type", move.Type.ToString());
            console.WriteProperty("Power", move.Power.ToString());
            console.WriteProperty("Accuracy", $"{move.Accuracy}%");
            console.WriteProperty("MovementFlags", move.MovementFlags.ToString());
            console.WriteProperty("Range", move.Range.ToString());
            console.WriteProperty("Effects", $"{move.Effect0} {RenderQuantityForMoveEffect(move.Effect0, move.Effect0Chance)}/ {move.Effect1} {RenderQuantityForMoveEffect(move.Effect1, move.Effect1Chance)}");
            console.WriteProperty("Unused Effect Duplicates", $"{move.Effect2} {RenderQuantityForMoveEffect(move.Effect2, move.Effect2Chance)}/ {move.Effect3} {RenderQuantityForMoveEffect(move.Effect3, move.Effect3Chance)}");
        }

        public static void Render(this IConsole console, IAbility ability, AbilityId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", ability.Name);
            console.WriteProperty("Effect1", $"{ability.Effect1} ({ability.Effect1Amount})");
            console.WriteProperty("Effect2", $"{ability.Effect2} ({ability.Effect2Amount})");
        }

        public static void Render(this IConsole console, IWarriorSkill saihai, WarriorSkillId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", saihai.Name);
            console.WriteProperty("Effect1", $"{saihai.Effect1} ({saihai.Effect1Amount})");
            console.WriteProperty("Effect2", $"{saihai.Effect2} ({saihai.Effect2Amount})");
            console.WriteProperty("Effect3", $"{saihai.Effect3} ({saihai.Effect3Amount})");
            console.WriteProperty("Target", saihai.Target.ToString());
            console.WriteProperty("Duration", saihai.Duration.ToString());
        }

        public static void Render(this IConsole console, IGimmick gimmick, GimmickId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", gimmick.Name);
        }

        public static void Render(this IConsole console, IBuilding building, BuildingId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", building.Name);
            console.WriteProperty("Kingdom", building.Kingdom.ToString());
        }

        public static void Render(this IConsole console, IItem item, ItemId id)
        {
            console.WriteTitle($"{id} ({(int)id})");
            console.WriteProperty("Name", item.Name);
        }

    }
}
