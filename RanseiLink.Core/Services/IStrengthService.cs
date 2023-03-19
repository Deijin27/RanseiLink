using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services;

public interface IStrengthService
{
    int CalculatePokemonStrength(Pokemon pokemon, double link);
    int CalculateScenarioKingdomStrength(ScenarioId scenario, KingdomId kingdom, int army);
    int CalculateScenarioPokemonStrength(ScenarioId scenario, int ScenarioPokemon);
    int CalculateScenarioPokemonStrength(ScenarioPokemon scenarioPokemon);
    int CalculateScenarioWarriorStrength(ScenarioId scenario, ScenarioWarrior scenarioWarrior);
}
