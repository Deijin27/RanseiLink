using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomizerPlugin;

internal class Randomizer
{
    private PokemonId[] pokemonIds = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
    private AbilityId[] abilityIds = EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray();
    private TypeId[] typeIds = EnumUtil.GetValues<TypeId>().ToArray();
    private MoveId[] moveIds = EnumUtil.GetValues<MoveId>().ToArray();
    private WarriorId[] warriorIds = EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray();
    private ScenarioId[] scenarioIds = EnumUtil.GetValues<ScenarioId>().ToArray();
    private MoveRangeId[] moveRangeIds = EnumUtil.GetValues<MoveRangeId>().ToArray();
    private MoveAnimationId[] moveAnimationIds = EnumUtil.GetValues<MoveAnimationId>().Where(i => i != MoveAnimationId.CausesASoftlock_DontUse).ToArray();

    private Dictionary<PokemonId, IPokemon> _allPokemon;
    private Dictionary<ScenarioId, Dictionary<int, IScenarioPokemon>> _allScenarioPokemon;
    private Dictionary<ScenarioId, Dictionary<int, IScenarioWarrior>> _allScenarioWarriors;
    private Dictionary<MoveId, IMove> _allMoves;
    private Dictionary<MoveRangeId, IMoveRange> _allMoveRanges;

    private IModServiceContainer _dataService;

    private RandomizationOptionForm options;

    private Random random;

    private IScenarioPokemon PlayersScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][(int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][0].ScenarioPokemon];
    private IScenarioPokemon OichisScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][(int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][2].ScenarioPokemon];
    private IScenarioPokemon KorokusScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][(int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][58].ScenarioPokemon];
    private IScenarioPokemon NagayasusScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][(int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][66].ScenarioPokemon];

    private readonly HashSet<MoveEffectId> invalidEffects = new()
    {
        MoveEffectId.VanishesAndHitsStartOfNextTurn,
        MoveEffectId.VanishesWithTargetAndHitsStartOfNextTurn,
        MoveEffectId.HitsStartOfTurnAfterNext,
    };

    public void Run(IPluginContext context)
    {
        var optionService = context.ServiceContainer.Resolve<IPluginService>();
        options = new RandomizationOptionForm();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.ServiceContainer.Resolve<IDialogService>();
        _dataService = context.ServiceContainer.Resolve<DataServiceFactory>()(context.ActiveMod);

        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Initializing Randomizer..."));
            Init();
            progress.Report(new ProgressInfo(Progress:15, StatusText:"Randomizing..."));

            Randomize();

            if (options.SoftlockMinimization)
            {
                while (!IsValid())
                {
                    Randomize();
                }
            }

            RandomizeNoValidationNeeded();

            if (options.AllMaxLinkValue > 0)
            {
                progress.Report(new ProgressInfo(Progress: 70, StatusText: "Handling max link values..."));
                HandleMaxLink();
            }

            progress.Report(new ProgressInfo(Progress: 85, StatusText: "Saving randomized data..."));
            Save();


            progress.Report(new ProgressInfo(Progress: 100, StatusText: "Randomization Complete!"));

        });
    }

    /// <summary>
    /// Load all the required stuff into memory
    /// </summary>
    private void Init()
    {
        _allPokemon = new();
        using (var pokemonService = _dataService.Pokemon.Disposable())
        {
            foreach (PokemonId pokemonId in pokemonIds)
            {
                _allPokemon[pokemonId] = pokemonService.Retrieve(pokemonId);
            }
        }

        _allMoves = new();
        using (var moveService = _dataService.Move.Disposable())
        {
            foreach (MoveId moveId in moveIds)
            {
                _allMoves[moveId] = moveService.Retrieve(moveId);
            }
        }

        _allMoveRanges = new();
        using (var moveRangeService = _dataService.MoveRange.Disposable())
        {
            foreach (MoveRangeId moveRangeId in moveRangeIds)
            {
                _allMoveRanges[moveRangeId] = moveRangeService.Retrieve(moveRangeId);
            }
        }

        _allScenarioPokemon = new();
        using (var scenarioPokemonService = _dataService.ScenarioPokemon.Disposable())
        {
            foreach (ScenarioId scenarioId in scenarioIds)
            {
                Dictionary<int, IScenarioPokemon> dict = new();
                for (int j = 0; j < Constants.ScenarioPokemonCount; j++)
                {
                    dict[j] = scenarioPokemonService.Retrieve(scenarioId, j);
                }
                _allScenarioPokemon[scenarioId] = dict;
            }
        }

        _allScenarioWarriors = new();
        using (var scenarioWarriorService = _dataService.ScenarioWarrior.Disposable())
        {
            foreach (ScenarioId scenarioId in scenarioIds)
            {
                Dictionary<int, IScenarioWarrior> dict = new();
                for (int j = 0; j < Constants.ScenarioWarriorCount; j++)
                {
                    dict[j] = scenarioWarriorService.Retrieve(scenarioId, j);
                }
                _allScenarioWarriors[scenarioId] = dict;
            }
        }

        if (options.AvoidDummyAbilities)
        {
            abilityIds = abilityIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }
        if (options.AvoidDummyMoves)
        {
            moveIds = moveIds.Where(i => !i.ToString().StartsWith("dummy")).ToArray();
        }

        random = new Random(options.Seed.GetHashCode());
    }

    /// <summary>
    ///  Save only things that will change from randomization
    /// </summary>
    private void Save()
    {
        using var pokemonService = _dataService.Pokemon.Disposable();
        using var scenarioPokemonService = _dataService.ScenarioPokemon.Disposable();
        using var scenarioWarriorService = _dataService.ScenarioWarrior.Disposable();
        using var moveService = _dataService.Move.Disposable();

        foreach (var (id, pokemon) in _allPokemon)
        {
            pokemonService.Save(id, pokemon);
        }

        foreach (var (scenarioId, dict) in _allScenarioPokemon)
        {
            foreach (var (scenarioPokemonId, scenarioPokemon) in dict)
            {
                scenarioPokemonService.Save(scenarioId, scenarioPokemonId, scenarioPokemon);
            }
        }

        foreach (var (scenarioId, dict) in _allScenarioWarriors)
        {
            foreach (var (scenarioWarriorId, scenarioWarrior) in dict)
            {
                scenarioWarriorService.Save(scenarioId, scenarioWarriorId, scenarioWarrior);
            }
        }

        foreach (var (id, move) in _allMoves)
        {
            moveService.Save(id, move);
        }
    }

    /// <summary>
    /// Randomization that requires validation
    /// </summary>
    private void Randomize()
    {
        if (options.Abilities || options.Types || options.Moves)
        {
            foreach (IPokemon poke in _allPokemon.Values)
            {
                if (options.Abilities)
                {
                    poke.Ability1 = random.Choice(abilityIds);
                    poke.Ability2 = random.Choice(abilityIds);
                    poke.Ability3 = random.Choice(abilityIds);
                }
                if (options.Types)
                {
                    poke.Type1 = random.Choice(typeIds);
                    poke.Type2 = options.PreventSameType
                        ? random.Choice(typeIds.Where(i => i != poke.Type1).ToArray())
                        : random.Choice(typeIds);

                }
                if (options.Moves)
                {
                    poke.Move = random.Choice(moveIds);
                }
            }
        }

        if (options.ScenarioPokemon)
        {
            foreach (IScenarioPokemon sp in _allScenarioPokemon.Values.SelectMany(i => i.Values))
            {
                sp.Pokemon = random.Choice(pokemonIds);
                var pk = _allPokemon[sp.Pokemon];
                var abilities = new[] { pk.Ability1, pk.Ability2, pk.Ability3 }.Where(i => i != AbilityId.NoAbility).ToArray();
                sp.Ability = abilities.Any() ? random.Choice(abilities) : AbilityId.NoAbility;
            }
        }
    }

    /// <summary>
    /// Randomization that doesn't require validation
    /// </summary>
    private void RandomizeNoValidationNeeded()
    {
        if (options.Warriors)
        {
            foreach (IScenarioWarrior sw in _allScenarioWarriors.Values.SelectMany(i => i.Values))
            {
                sw.Warrior = random.Choice(warriorIds);
            }
        }

        if (options.MoveAnimations)
        {
            foreach (var move in _allMoves.Values)
            {
                move.StartupAnimation = random.Choice(moveAnimationIds);
                move.ProjectileAnimation = random.Choice(moveAnimationIds);
                move.ImpactAnimation = random.Choice(moveAnimationIds);
            }
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
        if (_allPokemon[PlayersScenarioPokemon.Pokemon].MovementRange < 3)
        {
            return false;
        }

        if (_allPokemon[OichisScenarioPokemon.Pokemon].MovementRange < 2)
        {
            return false;
        }

        if (_allPokemon[KorokusScenarioPokemon.Pokemon].MovementRange < 2)
        {
            return false;
        }

        if (_allPokemon[NagayasusScenarioPokemon.Pokemon].MovementRange < 2)
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

    private bool ValidateTutorialMatchup(IScenarioPokemon attackingScenarioPokemon, IScenarioPokemon targetScenarioPokemon, bool oichisPokemon = false)
    {
        var attackingPokemon = _allPokemon[attackingScenarioPokemon.Pokemon];
        var targetPokemon = _allPokemon[targetScenarioPokemon.Pokemon];
        var move = _allMoves[attackingPokemon.Move];

        // Moves always hit during the tutorial, so this doesn't matter
        //if (move.Accuracy != 100)
        //{
        //    return false;
        //}
        if ((options.Moves || options.ScenarioPokemon) && move.Power < 30)
        {
            return false;
        }
        if (move.Power < 60 && targetPokemon.Resists(move.Type))
        {
            return false;
        }
        if (!_allMoveRanges[move.Range].GetInRange(1))
        {
            return false;
        }
        if (oichisPokemon && _allMoveRanges[move.Range].GetInRange(24))
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

        return true;
    }

    private readonly AbilityId[] MoldBreakerAbilitys = new[] { AbilityId.MoldBreaker, AbilityId.Teravolt, AbilityId.Turboblaze };

    private void HandleMaxLink()
    {
        using var maxLinkService = _dataService.MaxLink.Disposable();

        foreach (WarriorId wid in warriorIds)
        {
            var maxLinkTable = maxLinkService.Retrieve(wid);
            foreach (PokemonId pid in pokemonIds)
            {
                if (maxLinkTable.GetMaxLink(pid) < options.AllMaxLinkValue)
                {
                    maxLinkTable.SetMaxLink(pid, options.AllMaxLinkValue);
                }
            }
            maxLinkService.Save(wid, maxLinkTable);
        }
    }
}
