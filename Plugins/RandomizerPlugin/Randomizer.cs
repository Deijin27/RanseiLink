﻿using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomizerPlugin;

public class Randomizer
{
    private readonly PokemonId[] _pokemonIds = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
    private AbilityId[] _abilityIds = EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray();
    private readonly TypeId[] _typeIds = EnumUtil.GetValues<TypeId>().ToArray();
    private readonly TypeId[] _typeIdsExceptNoType = EnumUtil.GetValues<TypeId>().Where(i => i != TypeId.NoType).ToArray();
    private MoveId[] _moveIds = EnumUtil.GetValues<MoveId>().ToArray();
    private readonly WarriorId[] _warriorIds = EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray();
    private readonly MoveAnimationId[] _moveAnimationIds = EnumUtil.GetValues<MoveAnimationId>().Where(i => i != MoveAnimationId.CausesASoftlock_DontUse).ToArray();
    private WarriorSkillId[] _warriorSkillIds = EnumUtil.GetValues<WarriorSkillId>().ToArray();
    private readonly BattleConfigId[] _battleConfigIds = EnumUtil.GetValuesExceptDefaults<BattleConfigId>().ToArray();

    private IScenarioPokemonService _scenarioPokemonService;
    private IScenarioWarriorService _scenarioWarriorService;
    private IPokemonService _pokemonService;
    private IMoveService _moveService;
    private IMoveRangeService _moveRangeService;

    private RandomizationOptionForm _options;

    private Random random;

    private ScenarioPokemon PlayersScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(_scenarioWarriorService.Retrieve(0).Retrieve(0).GetScenarioPokemon(0));
    private ScenarioPokemon OichisScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(_scenarioWarriorService.Retrieve(0).Retrieve(2).GetScenarioPokemon(0));
    private ScenarioPokemon KorokusScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(_scenarioWarriorService.Retrieve(0).Retrieve(58).GetScenarioPokemon(0));
    private ScenarioPokemon NagayasusScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(_scenarioWarriorService.Retrieve(0).Retrieve(66).GetScenarioPokemon(0));

    private readonly HashSet<MoveEffectId> invalidEffects = new()
    {
        MoveEffectId.VanishesAndHitsStartOfNextTurn,
        MoveEffectId.VanishesWithTargetAndHitsStartOfNextTurn,
        MoveEffectId.HitsStartOfTurnAfterNext,
    };

    public void Run(IPluginContext context, RandomizationOptionForm options, IProgress<ProgressInfo> progress = null)
    {
        _options = options;
        progress.Report(new ProgressInfo("Initializing Randomizer..."));
        Init(context.Services);
        progress.Report(new ProgressInfo(Progress:15, StatusText:"Randomizing..."));

        Randomize();

        if (options.SoftlockMinimization)
        {
            while (!IsValid())
            {
                Randomize();
            }
        }

        RandomizeNoValidationNeeded(context.Services);

        if (options.AllMaxLinkValue > 0)
        {
            progress.Report(new ProgressInfo(Progress: 70, StatusText: "Handling max link values..."));
            HandleMaxLink(context.Services.Get<IMaxLinkService>());
        }

        progress.Report(new ProgressInfo(Progress: 85, StatusText: "Saving randomized data..."));
        Save();


        progress.Report(new ProgressInfo(Progress: 100, StatusText: "Randomization Complete!"));
    }

    /// <summary>
    /// Load all the required stuff into memory
    /// </summary>
    private void Init(IServiceGetter services)
    {
        _moveService = services.Get<IMoveService>();
        _moveRangeService = services.Get<IMoveRangeService>();
        _pokemonService = services.Get<IPokemonService>();
        _scenarioPokemonService = services.Get<IScenarioPokemonService>();
        _scenarioWarriorService = services.Get<IScenarioWarriorService>();

        _moveRangeService = services.Get<IMoveRangeService>();

        if (_options.AvoidDummyAbilities)
        {
            _abilityIds = _abilityIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }
        if (_options.AvoidDummyMoves)
        {
            _moveIds = _moveIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }
        if (_options.AvoidDummyWarriorSkills)
        {
            _warriorSkillIds = _warriorSkillIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }

        random = new Random(_options.Seed.GetHashCode());
    }

    /// <summary>
    ///  Save only things that will change from randomization
    /// </summary>
    private void Save()
    {
        _pokemonService.Save();
        _moveService.Save();
        _scenarioPokemonService.Save();
        _scenarioWarriorService.Save();
    }

    /// <summary>
    /// Randomization that requires validation
    /// </summary>
    private void Randomize()
    {
        if (_options.Abilities || _options.Types || _options.Moves)
        {
            foreach (Pokemon poke in _pokemonService.Enumerate())
            {
                if (_options.Abilities)
                {
                    poke.Ability1 = random.Choice(_abilityIds);
                    poke.Ability2 = random.Choice(_abilityIds);
                    poke.Ability3 = random.Choice(_abilityIds);
                }
                if (_options.Types)
                {
                    poke.Type1 = random.Choice(_typeIdsExceptNoType);
                    poke.Type2 = _options.PreventSameType
                        ? random.Choice(_typeIds.Where(i => i != poke.Type1).ToArray())
                        : random.Choice(_typeIds);

                }
                if (_options.Moves)
                {
                    poke.Move = random.Choice(_moveIds);
                }
            }
        }

        if (_options.ScenarioPokemon)
        {
            foreach (ScenarioPokemon sp in _scenarioPokemonService.Enumerate().SelectMany(i => i.Enumerate()))
            {
                sp.Pokemon = random.Choice(_pokemonIds);
                var pk = _pokemonService.Retrieve((int)sp.Pokemon);
                var abilities = new[] { pk.Ability1, pk.Ability2, pk.Ability3 }.Where(i => i != AbilityId.NoAbility).ToArray();
                sp.Ability = abilities.Any() ? random.Choice(abilities) : AbilityId.NoAbility;
            }
        }
    }

    /// <summary>
    /// Randomization that doesn't require validation
    /// </summary>
    private void RandomizeNoValidationNeeded(IServiceGetter services)
    {
        if (_options.Warriors)
        {
            foreach (ScenarioWarrior sw in _scenarioWarriorService.Enumerate().SelectMany(i => i.Enumerate()))
            {
                sw.Warrior = random.Choice(_warriorIds);
            }
        }

        if (_options.MoveAnimations)
        {
            foreach (var move in _moveService.Enumerate())
            {
                move.StartupAnimation = random.Choice(_moveAnimationIds);
                move.ProjectileAnimation = random.Choice(_moveAnimationIds);
                move.ImpactAnimation = random.Choice(_moveAnimationIds);
            }
        }

        if (_options.WarriorSkills)
        {
            var baseWarriorService = services.Get<IBaseWarriorService>();
            foreach (var warrior in baseWarriorService.Enumerate())
            {
                warrior.Skill = random.Choice(_warriorSkillIds);
            }
            baseWarriorService.Save();
        }

        if (_options.StatRandomMode == StatRandomizationMode.Shuffle)
        {
            foreach (var pokemon in _pokemonService.Enumerate())
            {
                var statArray = new[] { pokemon.Hp, pokemon.Atk, pokemon.Def, pokemon.Spe };
                RandomShuffle(random, statArray);
                pokemon.Hp = statArray[0];
                pokemon.Atk = statArray[1];
                pokemon.Def = statArray[2];
                pokemon.Spe = statArray[3];
            }
        }
        else if (_options.StatRandomMode == StatRandomizationMode.Range)
        {
            var min = _options.StatRangeMin;
            var max = _options.StatRangeMax + 1;
            foreach (var pokemon in _pokemonService.Enumerate())
            {
                pokemon.Hp = random.Next(min, max);
                pokemon.Atk = random.Next(min, max);
                pokemon.Def = random.Next(min, max);
                pokemon.Spe = random.Next(min, max);
            }
        }

        if (_options.DiscoLights)
        {
            var battleConfigService = services.Get<IBattleConfigService>();
            foreach (var battleConfig in battleConfigService.Enumerate())
            {
                battleConfig.UpperAtmosphereColor = RandomColor(random);
                battleConfig.MiddleAtmosphereColor = RandomColor(random);
                battleConfig.LowerAtmosphereColor = RandomColor(random);
            }
            battleConfigService.Save();
        }

        if (_options.BattleMaps)
        {
            var kingdomService = services.Get<IKingdomService>();
            foreach (var kingdom in kingdomService.Enumerate().Skip(1)) // skip aurora because it will break tutorial
            {
                kingdom.BattleConfig = random.Choice(_battleConfigIds);
            }
            kingdomService.Save();

            var buildingService = services.Get<IBuildingService>();
            foreach (var building in buildingService.Enumerate().Where(x => x.Function == BuildingFunctionId.Battle))
            {
                building.BattleConfig1 = random.Choice(_battleConfigIds);
                building.BattleConfig2 = random.Choice(_battleConfigIds);
                building.BattleConfig3 = random.Choice(_battleConfigIds);
            }
            buildingService.Save();
        }
    }

    /// <summary>
    /// Validate current randomization to minimize softlock chance.
    /// </summary>
    /// <returns>False if a softlock will definitely occur</returns>
    private bool IsValid()
    {
        if (!ValidateTutorial())
        {
            return false;
        }

        return true;
    }

    private bool ValidateTutorial()
    {
        if (_pokemonService.Retrieve((int)PlayersScenarioPokemon.Pokemon).MovementRange < 3)
        {
            return false;
        }

        if (_pokemonService.Retrieve((int)OichisScenarioPokemon.Pokemon).MovementRange < 2)
        {
            return false;
        }

        if (_pokemonService.Retrieve((int)KorokusScenarioPokemon.Pokemon).MovementRange < 2)
        {
            return false;
        }

        if (_pokemonService.Retrieve((int)NagayasusScenarioPokemon.Pokemon).MovementRange < 2)
        {
            return false;
        }

        if (!ValidateTutorialMatchup(PlayersScenarioPokemon, KorokusScenarioPokemon))
        {
            return false;
        }

        if (!ValidateTutorialMatchup(OichisScenarioPokemon, NagayasusScenarioPokemon, true))
        {
            return false;
        }

        return true;
    }

    private static HashSet<MoveEffectId> _multiHitEffects = new HashSet<MoveEffectId>
    { MoveEffectId.Hits2Times, MoveEffectId.Hits2To3Times, MoveEffectId.Hits2To5Times, MoveEffectId.Hits4To5Times, MoveEffectId.Hits5Times };

    private bool IsMultiHit(Move move)
    {
        foreach (var effect in new[] { move.Effect1, move.Effect2 })
        {
            if (_multiHitEffects.Contains(effect))
            {
                return true;
            }
        }
        return false;
    }

    private bool ValidateTutorialMatchup(ScenarioPokemon attackingScenarioPokemon, ScenarioPokemon targetScenarioPokemon, bool oichisPokemon = false)
    {
        var attackingPokemon = _pokemonService.Retrieve((int)attackingScenarioPokemon.Pokemon);
        var targetPokemon = _pokemonService.Retrieve((int)targetScenarioPokemon.Pokemon);
        var move = _moveService.Retrieve((int)attackingPokemon.Move);

        if ((_options.Moves || _options.ScenarioPokemon))
        {
            // Moves always hit during the tutorial, so this doesn't matter
            //if (move.Accuracy != 100)
            //{
            //    return false;
            //}
            bool isMultiHit = IsMultiHit(move);
            if (!isMultiHit)
            {
                if (move.Power < 30)
                {
                    return false;
                }
                if (move.Power < 60 && targetPokemon.Resists(move.Type) && !isMultiHit)
                {
                    return false;
                }
            }
            if (!_moveRangeService.Retrieve((int)move.Range).GetInRange(1))
            {
                return false;
            }
            if (oichisPokemon && _moveRangeService.Retrieve((int)move.Range).GetInRange(24))
            {
                return false;
            }
            if (new[] { move.Effect1, move.Effect2 }.Any(i => invalidEffects.Contains(i)))
            {
                return false;
            }
            if (targetPokemon.IsImmuneTo(move.Type))
            {
                return false;
            }
        }
        
        if (_options.Abilities || _options.ScenarioPokemon)
        {
            if (targetScenarioPokemon.Ability == AbilityId.Sturdy)
            {
                return false;
            }
            if (!MoldBreakerAbilitys.Contains(attackingScenarioPokemon.Ability))
            {
                switch (targetScenarioPokemon.Ability)
                {
                    case AbilityId.Levitate:
                        if (move.Type == TypeId.Ground)
                            return false;
                        break;
                    case AbilityId.WaterAbsorb:
                        if (move.Type == TypeId.Water)
                            return false;
                        break;
                    case AbilityId.FlashFire:
                        if (move.Type == TypeId.Fire)
                            return false;
                        break;
                    case AbilityId.VoltAbsorb:
                    case AbilityId.Lightningrod:
                    case AbilityId.MotorDrive:
                        if (move.Type == TypeId.Electric)
                            return false;
                        break;
                }
            }
        }
        
        return true;
    }

    private readonly AbilityId[] MoldBreakerAbilitys = new[] { AbilityId.MoldBreaker, AbilityId.Teravolt, AbilityId.Turboblaze };

    private void HandleMaxLink(IMaxLinkService maxLinkService)
    {
        foreach (var maxLinkTable in maxLinkService.Enumerate())
        {
            foreach (PokemonId pid in _pokemonIds)
            {
                if (maxLinkTable.GetMaxLink(pid) < _options.AllMaxLinkValue)
                {
                    maxLinkTable.SetMaxLink(pid, _options.AllMaxLinkValue);
                }
            }
        }

        maxLinkService.Save();
    }

    private static void RandomShuffle<T>(Random random, T[] values)
    {
        int n = values.Length;
        while (n > 1)
        {
            int m = random.Next(n--);
            (values[n], values[m]) = (values[m], values[n]);
        }
    }

    private static RanseiLink.Core.Graphics.Rgb15 RandomColor(Random random)
    {
        return new RanseiLink.Core.Graphics.Rgb15(random.Next(32), random.Next(32), random.Next(32));
    }
}
