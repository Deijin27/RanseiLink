using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services.Concrete;

public partial class ModServiceContainer(IServiceGetter modServices) : IModServiceContainer
{
    private readonly Lazy<IScenarioPokemonService> _scenarioPokemon = new(modServices.Get<IScenarioPokemonService>);
    private readonly Lazy<IScenarioWarriorService> _scenarioWarrior = new(modServices.Get<IScenarioWarriorService>);
    private readonly Lazy<IScenarioAppearPokemonService> _scenarioAppearPokemon = new(modServices.Get<IScenarioAppearPokemonService>);
    private readonly Lazy<IScenarioKingdomService> _scenarioKingdom = new(modServices.Get<IScenarioKingdomService>);
    private readonly Lazy<IScenarioBuildingService> _scenarioBuilding = new(modServices.Get<IScenarioBuildingService>);
    private readonly Lazy<IMsgBlockService> _msg = new(modServices.Get<IMsgBlockService>);
    private readonly Lazy<IMapService> _map = new(modServices.Get<IMapService>);
    private readonly Lazy<IOverrideDataProvider> _overrideSpriteProvider = new(modServices.Get<IOverrideDataProvider>);
    public IScenarioPokemonService ScenarioPokemon => _scenarioPokemon.Value;
    public IScenarioWarriorService ScenarioWarrior => _scenarioWarrior.Value;
    public IScenarioAppearPokemonService ScenarioAppearPokemon => _scenarioAppearPokemon.Value;
    public IScenarioKingdomService ScenarioKingdom => _scenarioKingdom.Value;
    public IScenarioBuildingService ScenarioBuilding => _scenarioBuilding.Value;
    public IMsgBlockService Msg => _msg.Value;
    public IMapService Map => _map.Value;
    public IOverrideDataProvider OverrideSpriteProvider => _overrideSpriteProvider.Value;
} 
