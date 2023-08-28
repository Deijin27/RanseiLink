using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftlockCheckerPlugin;

internal class SoftlockChecker
{
    private IScenarioPokemonService _scenarioPokemonService;
    private IScenarioWarriorService _scenarioWarriorService;
    private IPokemonService _pokemonService;
    private IMoveService _moveService;
    private IMoveRangeService _moveRangeService;
    private IMaxLinkService _maxLinkService;
    private IBaseWarriorService _baseWarriorService;

    private int PlayersScenarioPokemonId => _scenarioWarriorService.Retrieve(0).Retrieve(0).GetScenarioPokemon(0);
    private int OichisScenarioPokemonId => _scenarioWarriorService.Retrieve(0).Retrieve(2).GetScenarioPokemon(0);
    private int KorokusScenarioPokemonId => _scenarioWarriorService.Retrieve(0).Retrieve(58).GetScenarioPokemon(0);
    private int NagayasusScenarioPokemonId => _scenarioWarriorService.Retrieve(0).Retrieve(66).GetScenarioPokemon(0);

    private ScenarioPokemon PlayersScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(PlayersScenarioPokemonId);
    private ScenarioPokemon OichisScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(OichisScenarioPokemonId);
    private ScenarioPokemon KorokusScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(KorokusScenarioPokemonId);
    private ScenarioPokemon NagayasusScenarioPokemon => _scenarioPokemonService.Retrieve(0).Retrieve(NagayasusScenarioPokemonId);

    /// <summary>
    /// Load all the required stuff into memory
    /// </summary>
    private void Init(IServiceGetter services)
    {
        _baseWarriorService = services.Get<IBaseWarriorService>();
        _moveService = services.Get<IMoveService>();
        _moveRangeService = services.Get<IMoveRangeService>();
        _pokemonService = services.Get<IPokemonService>();
        _scenarioPokemonService = services.Get<IScenarioPokemonService>();
        _scenarioWarriorService = services.Get<IScenarioWarriorService>();
        _maxLinkService = services.Get<IMaxLinkService>();

        _moveRangeService = services.Get<IMoveRangeService>();
    }

    private IDialogService _dialogService;

    public async Task Run(IPluginContext context)
    {
        _reportBuilder = new();
        Init(context.Services);
        Validate();
        _dialogService = context.Services.Get<IDialogService>();
        await NotifyUserIfNecessary();
    }

    /// <summary>
    /// If any softlocks exist, notify the user of them
    /// </summary>
    private async Task NotifyUserIfNecessary()
    {
        int totalCount = _guaranteed.Count + _conditional.Count + _probable.Count + _probableConditional.Count + _suggestion.Count;
        if (totalCount <= 0)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                "Validation Passed",
                "There is no known softlock causes in your mod!"
                ));
            return;
        }

        string result = new StringBuilder()
            .AppendLine($"Total softlock/crash causes found: {totalCount}")
            .AppendLine()
            .AppendLine($"Guaranteed: {_guaranteed.Count}")
            .AppendLine($"Conditional: {_conditional.Count}")
            .AppendLine($"Probable: {_probable.Count}")
            .AppendLine($"ProbableConditional: {_probableConditional.Count}")
            .AppendLine($"Suggestion: {_suggestion.Count}")
            .AppendLine()
            .AppendLine(_reportBuilder.ToString())
            .ToString();

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, result);
        var proc = Process.Start("notepad.exe", tempFile);
        // Wait for idle stopped working after windows 11 notpad update, so have to do this hack
        Thread.Sleep(1000);
        File.Delete(tempFile);
    }

    private StringBuilder _reportBuilder;

    private HashSet<string> _guaranteed = new();
    private HashSet<string> _conditional = new();
    private HashSet<string> _probable = new();
    private HashSet<string> _probableConditional = new();
    private HashSet<string> _suggestion = new();

    /// <summary>
    /// A softlock that is guaranteed to happen to someone in their playthrough
    /// </summary>
    /// <param name="description"></param>
    private void ReportGuaranteed(string description)
    {
        if (_guaranteed.Contains(description))
        {
            return;
        }
        _guaranteed.Add(description);
        _reportBuilder.Append("[Guaranteed] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is guaranteed to happen to someone if they do a certain thing
    /// </summary>
    /// <param name="description"></param>
    private void ReportConditional(string description)
    {
        if (_conditional.Contains(description))
        {
            return;
        }
        _conditional.Add(description);
        _reportBuilder.Append("[Conditional] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is possible to happen, but not guaranteed. And hte user will always run into this chance
    /// </summary>
    /// <param name="description"></param>
    private void ReportProbable(string description)
    {
        if (_probable.Contains(description))
        {
            return;
        }
        _probable.Add(description);
        _reportBuilder.Append("[Probable] ");
        _reportBuilder.AppendLine(description);
    }
    /// <summary>
    /// A softlock that is possible to happen, but not guaranteed. The user will only encounter this chance if they do a certain thing
    /// </summary>
    /// <param name="description"></param>
    private void ReportProbableConditional(string description)
    {
        if (_probableConditional.Contains(description))
        {
            return;
        }
        _probableConditional.Add(description);
        _reportBuilder.Append("[ProbableConditional] ");
        _reportBuilder.AppendLine(description);
    }

    private void ReportSuggestion(string description)
    {
        if (_suggestion.Contains(description))
        {
            return;
        }
        _suggestion.Add(description);
        _reportBuilder.Append("[Suggestion] ");
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
        BeginReportSection("Max Link Validation");
        ValidateLinkPairs();
    }

    private void ValidateAbilities()
    {
        foreach (ScenarioId scenario in EnumUtil.GetValues<ScenarioId>())
        {
            int id = 0;
            foreach (var scenarioWarrior in _scenarioWarriorService.Retrieve((int)scenario).Enumerate())
            {
                if (scenarioWarrior.ScenarioPokemonIsDefault(0))
                {
                    continue;
                }
                var scenarioPokemon = _scenarioPokemonService.Retrieve((int)scenario).Retrieve(scenarioWarrior.GetScenarioPokemon(0));
                var pokemon = _pokemonService.Retrieve((int)scenarioPokemon.Pokemon);
                var abilities = new[] { pokemon.Ability1, pokemon.Ability2, pokemon.Ability3 };
                if (!abilities.Contains(scenarioPokemon.Ability))
                {
                    ReportConditional($"Scenario={scenario}, ScenarioWarrior={id}, ScenarioPokemon={scenarioWarrior.GetScenarioPokemon(0)}, Pokemon={pokemon.Name}: has ability {scenarioPokemon.Ability} which is not on of {scenarioPokemon.Pokemon}'s abilities "
                        + $"({pokemon.Ability1}, {pokemon.Ability2}, {pokemon.Ability3})");
                }
                id++;
            }
        }
    }

    private void GetEvolutions(PokemonId pokemonId, List<PokemonId> evolutions)
    {
        if (evolutions.Contains(pokemonId) || pokemonId == PokemonId.Default) // prevent infinite loops
        {
            return;
        }
        evolutions.Add(pokemonId);
        var pokemon = _pokemonService.Retrieve((int)pokemonId);
        foreach (var evolution in pokemon.Evolutions)
        {
            GetEvolutions(evolution, evolutions);
        }
    }

    private void GetEvolutions(PokemonId pokemonId, List<PokemonId> evolutions, Dictionary<PokemonId, PokemonId> evolvesFrom)
    {
        if (evolutions.Contains(pokemonId) || pokemonId == PokemonId.Default) // prevent infinite loops
        {
            return;
        }
        evolutions.Add(pokemonId);
        var pokemon = _pokemonService.Retrieve((int)pokemonId);
        foreach (var evolution in pokemon.Evolutions)
        {
            evolvesFrom[evolution] = pokemonId;
            GetEvolutions(evolution, evolutions, evolvesFrom);
        }
    }

    private void GetRankups(WarriorId warriorId, List<WarriorId> rankups)
    {
        if (rankups.Contains(warriorId) || warriorId == WarriorId.NoWarrior)
        {
            return;
        }
        rankups.Add(warriorId);
        var warrior = _baseWarriorService.Retrieve((int)warriorId);
        GetRankups(warrior.RankUp, rankups);
    }

    private void GetRankups(WarriorId warriorId, List<WarriorId> rankups, Dictionary<WarriorId, WarriorId> ranksUpFrom)
    {
        if (rankups.Contains(warriorId) || warriorId == WarriorId.NoWarrior)
        {
            return;
        }
        rankups.Add(warriorId);
        var warrior = _baseWarriorService.Retrieve((int)warriorId);
        ranksUpFrom[warrior.RankUp] = warriorId;
        GetRankups(warrior.RankUp, rankups, ranksUpFrom);
    }

    private void ValidateLinkPairs()
    {
        // Scenario-Independent
        foreach (var warrior in EnumUtil.GetValuesExceptDefaults<WarriorId>())
        {
            var rankupTree = new List<WarriorId>();
            var ranksUpFrom = new Dictionary<WarriorId, WarriorId>();
            GetRankups(warrior, rankupTree, ranksUpFrom);
            foreach (var rankup in rankupTree)
            {
                var warriorMaxLinks = _maxLinkService.Retrieve((int)rankup);

                foreach (var pokemonId in EnumUtil.GetValuesExceptDefaults<PokemonId>())
                {
                    // gather all the pokemon that evolve from the warriors starter pokemon
                    var evolutionTree = new List<PokemonId>();
                    var evolvesFrom = new Dictionary<PokemonId, PokemonId>();
                    GetEvolutions(pokemonId, evolutionTree, evolvesFrom);

                    foreach (var evolution in evolutionTree)
                    {
                        var link = warriorMaxLinks.GetMaxLink(evolution);

                        if (evolvesFrom.TryGetValue(evolution, out var evolvesFromPoke))
                        {
                            var minLinkRequired = warriorMaxLinks.GetMaxLink(evolvesFromPoke);
                            if (link < minLinkRequired)
                            {
                                var poke = _pokemonService.Retrieve((int)evolution);
                                ReportSuggestion($"Warrior {(int)rankup} ({_baseWarriorService.IdToName((int)rankup)}) has max link '{link}' with pokemon '{poke.Name}'"
                                    + $" which is less than the link value '{minLinkRequired}' with it's pre-evolution '{_pokemonService.Retrieve((int)evolvesFromPoke).Name}'");
                            }
                        }
                        if (ranksUpFrom.TryGetValue(rankup, out var ranksUpFromWarrior))
                        {
                            var xWarriorMinLink = _maxLinkService.Retrieve((int)ranksUpFromWarrior).GetMaxLink(evolution);

                            if (link < xWarriorMinLink)
                            {
                                var poke = _pokemonService.Retrieve((int)evolution);
                                ReportSuggestion($"Warrior {(int)rankup} ({_baseWarriorService.IdToName((int)rankup)}) has max link '{link}' with pokemon '{poke.Name}'"
                                    + $" which is less than the link value '{xWarriorMinLink}' with a lower rank of this warrior ({(int)ranksUpFromWarrior})");
                            }
                        }
                    }
                }
            }
        }


        // Scenario-Dependent
        foreach (ScenarioId scenario in EnumUtil.GetValues<ScenarioId>())
        {
            int id = 0;
            foreach (var scenarioWarrior in _scenarioWarriorService.Retrieve((int)scenario).Enumerate())
            {
                if (scenarioWarrior.ScenarioPokemonIsDefault(0) || scenarioWarrior.Warrior == WarriorId.NoWarrior)
                {
                    continue;
                }
                var scenarioPokemon = _scenarioPokemonService.Retrieve((int)scenario).Retrieve(scenarioWarrior.GetScenarioPokemon(0));

                var rankupTree = new List<WarriorId>();
                GetRankups(scenarioWarrior.Warrior, rankupTree);
                var pokemonId = scenarioPokemon.Pokemon;
                foreach (var rankup in rankupTree)
                {
                    var warriorMaxLinks = _maxLinkService.Retrieve((int)scenarioWarrior.Warrior);
                    
                    // gather all the pokemon that evolve from the warriors starter pokemon
                    var evolutionTree = new List<PokemonId>();
                    GetEvolutions(pokemonId, evolutionTree);

                    foreach (var evolution in evolutionTree)
                    {
                        var link = warriorMaxLinks.GetMaxLink(evolution);
                        if (pokemonId == scenarioPokemon.Pokemon && link == 0)
                        {
                            var poke = _pokemonService.Retrieve((int)evolution);
                            ReportSuggestion($"In scenario '{scenario}' warrior {(int)rankup} ({_baseWarriorService.IdToName((int)rankup)}) (ScenarioWarrior={id}) has max link 0 with pokemon '{poke.Name}'"
                                + " which is the, or an evolution of their scenario pokemon.");
                        }
                    }
                }

                id++;
            }
        }
    
    
    }

    private void ValidateMoves()
    {
        foreach (var move in _moveService.Enumerate())
        {
            ValidateMoveAnimation(move, move.StartupAnimation);
            ValidateMoveAnimation(move, move.ProjectileAnimation);
            ValidateMoveAnimation(move, move.ImpactAnimation);
        }
    }

    private void ValidateMoveAnimation(Move move, MoveAnimationId id)
    {
        if (id == MoveAnimationId.CausesASoftlock_DontUse)
        {
            ReportConditional($"The move {move.Name} has animation {id}. Although this animation looks cool, it causes a softlock :(");
        }
    }

    private void ValidateTutorial()
    {
        var playerPoke = PlayersScenarioPokemon.Pokemon;
        var playerPokeRange = _pokemonService.Retrieve((int)playerPoke).MovementRange;
        if (playerPokeRange < 3)
        {
            ReportGuaranteed($"The player's pokemon ({PlayersScenarioPokemon.Pokemon}) has a movement range of {playerPokeRange}. A range of at least is necessary in the tutorial.");
        }
        var oichiPokeRange = _pokemonService.Retrieve((int)OichisScenarioPokemon.Pokemon).MovementRange;
        if (oichiPokeRange < 2)
        {
            ReportGuaranteed($"Oichi's pokemon pokemon ({OichisScenarioPokemon.Pokemon}) has a movement range of {oichiPokeRange}. A range of at least is necessary in the tutorial.");
        }
        var korokuPokeRange = _pokemonService.Retrieve((int)KorokusScenarioPokemon.Pokemon).MovementRange;
        if (korokuPokeRange < 2)
        {
            ReportGuaranteed($"Koroku's pokemon ({KorokusScenarioPokemon.Pokemon}) has a movement range of {korokuPokeRange}. A range of at least is necessary in the tutorial.");
        }
        var nagayasuPokeRange = _pokemonService.Retrieve((int)NagayasusScenarioPokemon.Pokemon).MovementRange;
        if (nagayasuPokeRange < 2)
        {
            ReportGuaranteed($"Nagayasus's pokemon ({NagayasusScenarioPokemon.Pokemon}) has a movement range of {nagayasuPokeRange}. A range of at least is necessary in the tutorial.");
        }

        ValidateTutorialMatchup(PlayersScenarioPokemon, KorokusScenarioPokemon);

        ValidateTutorialMatchup(OichisScenarioPokemon, NagayasusScenarioPokemon);
    }

    private void ValidateTutorialMatchup(ScenarioPokemon attackingScenarioPokemon, ScenarioPokemon targetScenarioPokemon, bool oichisPokemon = false)
    {
        Pokemon attackingPokemon = _pokemonService.Retrieve((int)attackingScenarioPokemon.Pokemon);
        Pokemon targetPokemon = _pokemonService.Retrieve((int)targetScenarioPokemon.Pokemon);
        Move move = _moveService.Retrieve((int)attackingPokemon.Move);

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
        if (!_moveRangeService.Retrieve((int)move.Range).GetInRange(1))
        {
            ReportGuaranteed($"The move {attackingPokemon.Move} used by pokemon {attackingScenarioPokemon.Pokemon} has range {move.Range} which does not hit the square immediately ahead of the user. This causes a tutorial softlock.");
        }
        if (oichisPokemon && _moveRangeService.Retrieve((int)move.Range).GetInRange(24))
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
