using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftlockCheckerPlugin;

internal class SoftlockChecker
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
    private Dictionary<MoveRangeId, IAttackRange> _allMoveRanges;

    private IDataService _dataService;

    private int PlayersScenarioPokemonId => (int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][0].ScenarioPokemon;
    private int OichisScenarioPokemonId => (int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][2].ScenarioPokemon;
    private int KorokusScenarioPokemonId => (int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][58].ScenarioPokemon;
    private int NagayasusScenarioPokemonId => (int)_allScenarioWarriors[ScenarioId.TheLegendOfRansei][66].ScenarioPokemon;

    private IScenarioPokemon PlayersScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][PlayersScenarioPokemonId];
    private IScenarioPokemon OichisScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][OichisScenarioPokemonId];
    private IScenarioPokemon KorokusScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][KorokusScenarioPokemonId];
    private IScenarioPokemon NagayasusScenarioPokemon => _allScenarioPokemon[ScenarioId.TheLegendOfRansei][NagayasusScenarioPokemonId];

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
    }

    private IDialogService _dialogService;

    public void Run(IPluginContext context)
    {
        _reportBuilder = new();
        guaranteedCount = 0;
        conditionalCount = 0;
        probableCount = 0;
        probableConditionalCount = 0;
        _dataService = context.ServiceContainer.Resolve<DataServiceFactory>()(context.ActiveMod);
        Init();
        Validate();
        _dialogService = context.ServiceContainer.Resolve<IDialogService>();
        NotifyUserIfNecessary();
    }

    /// <summary>
    /// If any softlocks exist, notify the user of them
    /// </summary>
    private void NotifyUserIfNecessary()
    {
        int totalCount = guaranteedCount + conditionalCount + probableCount + probableConditionalCount;
        if (totalCount <= 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                "Validation Passed",
                "There is no known softlock causes in your mod!"
                ));
            return;
        }

        string result = new StringBuilder()
            .AppendLine($"Total softlock/crash causes found: {totalCount}")
            .AppendLine()
            .AppendLine($"Guaranteed: {guaranteedCount}")
            .AppendLine($"Conditional: {conditionalCount}")
            .AppendLine($"Probable: {probableCount}")
            .AppendLine($"ProbableConditional: {probableConditionalCount}")
            .AppendLine()
            .AppendLine(_reportBuilder.ToString())
            .ToString();

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, result);
        var proc = Process.Start("notepad.exe", tempFile);
        proc.WaitForInputIdle();
        File.Delete(tempFile);
    }

    private StringBuilder _reportBuilder;
    private int guaranteedCount = 0;
    private int conditionalCount = 0;
    private int probableCount = 0;
    private int probableConditionalCount = 0;

    /// <summary>
    /// A softlock that is guaranteed to happen to someone in their playthrough
    /// </summary>
    /// <param name="description"></param>
    private void ReportGuaranteed(string description)
    {
        guaranteedCount++;
        _reportBuilder.Append("[Guaranteed] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is guaranteed to happen to someone if they do a certain thing
    /// </summary>
    /// <param name="description"></param>
    private void ReportConditional(string description)
    {
        conditionalCount++;
        _reportBuilder.Append("[Conditional] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is possible to happen, but not guaranteed. And hte user will always run into this chance
    /// </summary>
    /// <param name="description"></param>
    private void ReportProbable(string description)
    {
        probableCount++;
        _reportBuilder.Append("[Probable] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is possible to happen, but not guaranteed. The user will only encounter this chance if they do a certain thing
    /// </summary>
    /// <param name="description"></param>
    private void ReportProbableConditional(string description)
    {
        probableConditionalCount++;
        _reportBuilder.Append("[ProbableConditional] ");
        _reportBuilder.AppendLine(description);
    }

    private void BeginReportSection(string name)
    {
        _reportBuilder.AppendLine("----------------------------------------------------------");
        _reportBuilder.AppendLine(name);
        _reportBuilder.AppendLine("----------------------------------------------------------");
    }

    private void Validate()
    {
        BeginReportSection("Tutorial Validation");
        ValidateTutorial();
        BeginReportSection("Ability Validation");
        ValidateAbilities();
        BeginReportSection("Move Validation");
        ValidateMoves();
    }

    private void ValidateAbilities()
    {
        foreach (var (scenario, scenarioWarriorDict) in _allScenarioWarriors)
        {
            foreach (var (id, scenarioWarrior) in scenarioWarriorDict)
            {
                var scenarioPokemon = _allScenarioPokemon[scenario][(int)scenarioWarrior.ScenarioPokemon];
                var pokemon = _allPokemon[scenarioPokemon.Pokemon];
                var abilities = new[] { pokemon.Ability1, pokemon.Ability2, pokemon.Ability3 };
                if (!abilities.Contains(scenarioPokemon.Ability))
                {
                    ReportConditional($"Scenario={scenario}, ScenarioWarrior={id}, ScenarioPokemon={scenarioWarrior.ScenarioPokemon}, Pokemon={scenarioPokemon.Pokemon}: has ability {scenarioPokemon.Ability} which is not on of {scenarioPokemon.Pokemon}'s abilities "
                        + $"({pokemon.Ability1}, {pokemon.Ability2}, {pokemon.Ability3})");
                }
            }
        }
    }

    private void ValidateMoves()
    {
        foreach (var (id, move) in _allMoves)
        {
            ValidateMoveAnimation(id, move.StartupAnimation);
            ValidateMoveAnimation(id, move.ProjectileAnimation);
            ValidateMoveAnimation(id, move.ImpactAnimation);
        }
    }

    private void ValidateMoveAnimation(MoveId move, MoveAnimationId id)
    {
        if (id == MoveAnimationId.CausesASoftlock_DontUse)
        {
            ReportConditional($"The move {move} has animation {id}. Although this animation looks cool, it causes a softlock :(");
        }
    }

    private void ValidateTutorial()
    {
        var playerPoke = PlayersScenarioPokemon.Pokemon;
        var playerPokeRange = _allPokemon[playerPoke].MovementRange;
        if (playerPokeRange < 3)
        {
            ReportGuaranteed($"The player's pokemon ({PlayersScenarioPokemon.Pokemon}) has a movement range of {playerPokeRange}. A range of at least is necessary in the tutorial.");
        }
        if (_allPokemon[OichisScenarioPokemon.Pokemon].MovementRange < 2)
        {
            ReportGuaranteed($"Oichi's pokemon pokemon ({OichisScenarioPokemon.Pokemon}) has a movement range of {playerPokeRange}. A range of at least is necessary in the tutorial.");
        }

        if (_allPokemon[KorokusScenarioPokemon.Pokemon].MovementRange < 2)
        {
            ReportGuaranteed($"Koroku's pokemon ({KorokusScenarioPokemon.Pokemon}) has a movement range of {playerPokeRange}. A range of at least is necessary in the tutorial.");
        }

        if (_allPokemon[NagayasusScenarioPokemon.Pokemon].MovementRange < 2)
        {
            ReportGuaranteed($"Nagayasus's pokemon ({NagayasusScenarioPokemon.Pokemon}) has a movement range of {playerPokeRange}. A range of at least is necessary in the tutorial.");
        }

        ValidateTutorialMatchup(PlayersScenarioPokemon, KorokusScenarioPokemon);

        ValidateTutorialMatchup(OichisScenarioPokemon, NagayasusScenarioPokemon);
    }

    private void ValidateTutorialMatchup(IScenarioPokemon attackingScenarioPokemon, IScenarioPokemon targetScenarioPokemon, bool oichisPokemon = false)
    {
        var attackingPokemon = _allPokemon[attackingScenarioPokemon.Pokemon];
        var targetPokemon = _allPokemon[targetScenarioPokemon.Pokemon];
        var move = _allMoves[attackingPokemon.Move];

        // moves always hit during the tutorial, so  this isn't necessary
        //if (move.Accuracy != 100)
        //{
        //    ReportProbable($"The move {attackingPokemon.Move} used by {attackingScenarioPokemon.Pokemon} does not have 100% accuracy (it has {move.Accuracy}%). If it misses it will softlock the tutorial.");
        //}
        if (move.Power < 30)
        {
            ReportProbable($"The move used by {attackingScenarioPokemon.Pokemon} is weak ({move.Power}), thus there's a risk it doesn't one shot the opponent in the tutorial");
        }
        if (move.Power < 60 && targetPokemon.Resists(move.Type))
        {
            ReportProbable($"The defending pokemon {targetScenarioPokemon.Pokemon} resists move {attackingPokemon.Move} used by {attackingScenarioPokemon.Pokemon}, thus there's a risk it doesn't one shot the opponent in the tutorial");
        }
        if (!_allMoveRanges[move.Range].GetInRange(1))
        {
            ReportGuaranteed($"The move {attackingPokemon.Move} used by pokemon {attackingScenarioPokemon.Pokemon} has range {move.Range} which does not hit the square immediately ahead of the user. This causes a tutorial softlock.");
        }
        if (oichisPokemon && _allMoveRanges[move.Range].GetInRange(24))
        {
            ReportProbable($"The move {attackingPokemon.Move} used by pokemon {attackingScenarioPokemon.Pokemon} has range {move.Range} thus may take out Koroku, skipping the player's turn. This causes the 'Back' button to be not work in battles.");
        }
        if (invalidEffects.Contains(move.Effect1))
        {
            ReportGuaranteed($"The move {attackingPokemon.Move} used by pokemon {attackingScenarioPokemon.Pokemon} has effect {move.Effect1} which is multi-turn. This causes a tutorial softlock.");
        }
        if (invalidEffects.Contains(move.Effect2))
        {
            ReportGuaranteed($"The move {attackingPokemon.Move} used by pokemon {attackingScenarioPokemon.Pokemon} has effect {move.Effect2} which is multi-turn. This causes a tutorial softlock.");
        }
        if (targetPokemon.IsImmuneTo(move.Type))
        {
            ReportGuaranteed($"The defending pokemon {targetScenarioPokemon.Pokemon} is immune to move {attackingPokemon.Move} used by {attackingScenarioPokemon.Pokemon}.");
        }
        if (targetScenarioPokemon.Ability == AbilityId.Sturdy)
        {
            ReportGuaranteed($"The defending pokemon {targetScenarioPokemon.Pokemon} has the ability Sturdy, which means it can not be one shot in the tutorial");
        }
        if (!MoldBreakerAbilitys.Contains(attackingScenarioPokemon.Ability))
        {
            void reportAbilityTypeImmunity(AbilityId abilityId, TypeId type) => ReportGuaranteed($"The defending pokemon {targetScenarioPokemon.Pokemon} has the ability {abilityId}, which means it is immune to the {type} type move {attackingPokemon.Move} used by {attackingScenarioPokemon.Pokemon}");
            switch (targetScenarioPokemon.Ability)
            {
                case AbilityId.Levitate:
                    if (move.Type == TypeId.Ground)
                        reportAbilityTypeImmunity(AbilityId.Levitate, TypeId.Ground);
                    break;
                case AbilityId.WaterAbsorb:
                    if (move.Type == TypeId.Water)
                        reportAbilityTypeImmunity(AbilityId.WaterAbsorb, TypeId.Water);
                    break;
                case AbilityId.FlashFire:
                    if (move.Type == TypeId.Fire)
                        reportAbilityTypeImmunity(AbilityId.FlashFire, TypeId.Fire);
                    break;
                case AbilityId.VoltAbsorb:
                    if (move.Type == TypeId.Electric)
                        reportAbilityTypeImmunity(AbilityId.VoltAbsorb, TypeId.Electric);
                    break;
                case AbilityId.Lightningrod:
                    if (move.Type == TypeId.Electric)
                        reportAbilityTypeImmunity(AbilityId.Lightningrod, TypeId.Electric);
                    break;
                case AbilityId.MotorDrive:
                    if (move.Type == TypeId.Electric)
                        reportAbilityTypeImmunity(AbilityId.MotorDrive, TypeId.Electric);
                    break;
            }
        }
    }

    private readonly HashSet<AbilityId> MoldBreakerAbilitys = new() { AbilityId.MoldBreaker, AbilityId.Teravolt, AbilityId.Turboblaze };

    private readonly HashSet<MoveEffectId> invalidEffects = new()
    {
        MoveEffectId.VanishesAndHitsStartOfNextTurn,
        MoveEffectId.VanishesWithTargetAndHitsStartOfNextTurn,
        MoveEffectId.HitsStartOfTurnAfterNext,
    };
}
