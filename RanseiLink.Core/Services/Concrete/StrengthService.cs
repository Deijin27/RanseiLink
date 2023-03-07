using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services.Concrete
{
    internal class StrengthService : IStrengthService
    {
        private readonly IScenarioPokemonService _scenarioPokemonService;
        private readonly IScenarioWarriorService _scenarioWarriorService;
        private readonly IPokemonService _pokemonService;

        public StrengthService(
            IScenarioPokemonService scenarioPokemonService,
            IScenarioWarriorService scenarioWarriorService,
            IPokemonService pokemonService)
        {
            _scenarioPokemonService = scenarioPokemonService;
            _scenarioWarriorService = scenarioWarriorService;
            _pokemonService = pokemonService;
        }

        public int CalculatePokemonStrength(Pokemon pokemon, double link)
        {
            return StrengthCalculator.CalculateStrength(pokemon.Hp, pokemon.Atk, pokemon.Def, pokemon.Spe, link);
        }

        public int CalculateScenarioPokemonStrength(ScenarioPokemon scenarioPokemon)
        {
            int pokemonId = (int)scenarioPokemon.Pokemon;
            if (!_pokemonService.ValidateId(pokemonId))
            {
                return 0;
            }
            var pokemon = _pokemonService.Retrieve(pokemonId);
            return CalculatePokemonStrength(pokemon, (double)LinkCalculator.CalculateLink(scenarioPokemon.Exp));
        }

        public int CalculateScenarioPokemonStrength(ScenarioId scenario, int ScenarioPokemon)
        {
            var sp = _scenarioPokemonService.Retrieve((int)scenario).Retrieve(ScenarioPokemon);
            return CalculateScenarioPokemonStrength(sp);
        }

        public int CalculateScenarioKingdomStrength(ScenarioId scenario, KingdomId kingdom, int army)
        {
            int strength = 0;
            foreach (var warrior in _scenarioWarriorService.Retrieve((int)scenario).Enumerate())
            {
                if (warrior.Kingdom == kingdom
                    && warrior.Army == army
                    && (warrior.Class == WarriorClassId.ArmyLeader || warrior.Class == WarriorClassId.ArmyMember)
                    )
                {
                    if (warrior.ScenarioPokemonIsDefault(0))
                    {
                        continue;
                    }
                    strength += CalculateScenarioPokemonStrength(scenario, warrior.GetScenarioPokemon(0));
                }
            }
            return strength;
        }

        public int CalculateScenarioWarriorStrength(ScenarioId scenario, ScenarioWarrior scenarioWarrior)
        {
            if (scenarioWarrior.ScenarioPokemonIsDefault(0))
            {
                return 0;
            }
            var spid = scenarioWarrior.GetScenarioPokemon(0);
            var sp = _scenarioPokemonService.Retrieve((int)scenario).Retrieve(spid);
            int pokemonId = (int)sp.Pokemon;
            if (!_pokemonService.ValidateId(pokemonId))
            {
                return 0;
            }
            var pokemon = _pokemonService.Retrieve(pokemonId);
            return CalculatePokemonStrength(pokemon, (double)LinkCalculator.CalculateLink(sp.Exp));
        }
    }
}
