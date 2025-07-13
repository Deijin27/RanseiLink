using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services;

/// <summary>
/// Use for lua scripts only
/// </summary>
public partial interface IModServiceContainer
{
    IScenarioPokemonService ScenarioPokemon { get; }
    IScenarioWarriorService ScenarioWarrior { get; }
    IScenarioAppearPokemonService ScenarioAppearPokemon { get; }
    IScenarioKingdomService ScenarioKingdom { get; }
    IScenarioBuildingService ScenarioBuilding { get; }
    IMsgBlockService Msg { get; }
    IMapService Map { get; }
    IOverrideDataProvider OverrideSpriteProvider { get; }
}