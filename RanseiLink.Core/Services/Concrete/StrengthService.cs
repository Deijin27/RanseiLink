using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services.Concrete;

internal class StrengthService(
    IScenarioPokemonService scenarioPokemonService,
    IScenarioWarriorService scenarioWarriorService,
    IPokemonService pokemonService) : IStrengthService
{
    public int CalculatePokemonStrength(Pokemon pokemon, double link)
    {
        return MiscUtil.CalculateStrength(pokemon.Hp, pokemon.Atk, pokemon.Def, pokemon.Spe, link);
    }

    public int CalculateScenarioPokemonStrength(ScenarioPokemon scenarioPokemon)
    {
        int pokemonId = (int)scenarioPokemon.Pokemon;
        if (!pokemonService.ValidateId(pokemonId))
        {
            return 0;
        }
        var pokemon = pokemonService.Retrieve(pokemonId);
        return CalculatePokemonStrength(pokemon, (double)LinkCalculator.CalculateLink(scenarioPokemon.Exp));
    }

    public int CalculateScenarioPokemonStrength(ScenarioId scenario, int ScenarioPokemon)
    {
        var sp = scenarioPokemonService.Retrieve((int)scenario).Retrieve(ScenarioPokemon);
        return CalculateScenarioPokemonStrength(sp);
    }

    public int CalculateScenarioKingdomStrength(ScenarioId scenario, KingdomId kingdom, int army)
    {
        int strength = 0;
        foreach (var warrior in scenarioWarriorService.Retrieve((int)scenario).Enumerate())
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
        var sp = scenarioPokemonService.Retrieve((int)scenario).Retrieve(spid);
        int pokemonId = (int)sp.Pokemon;
        if (!pokemonService.ValidateId(pokemonId))
        {
            return 0;
        }
        var pokemon = pokemonService.Retrieve(pokemonId);
        return CalculatePokemonStrength(pokemon, (double)LinkCalculator.CalculateLink(sp.Exp));
    }
}
