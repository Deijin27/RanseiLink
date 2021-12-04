using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate ScenarioPokemonSelectorViewModel ScenarioPokemonSelectorViewModelFactory(IScenarioPokemonService service);

public class ScenarioPokemonSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioPokemon, ScenarioPokemonViewModel>
{
    private readonly ScenarioPokemonViewModelFactory _factory;
    private readonly IScenarioPokemonService _service;

    public ScenarioPokemonSelectorViewModel(IServiceContainer container, IScenarioPokemonService contextualDataService)
        : base(container, 0, 199)
    {
        _factory = container.Resolve<ScenarioPokemonViewModelFactory>();
        _service = contextualDataService;
        Init();
    }

    protected override ScenarioPokemonViewModel NewViewModel(ScenarioId scenarioId, IScenarioPokemon model) => _factory(model);

    protected override IScenarioPokemon RetrieveModel(ScenarioId scenario, uint index)
    {
        return _service.Retrieve(scenario, (int)index);
    }

    protected override void SaveModel(ScenarioId scenario, uint index, IScenarioPokemon model)
    {
        _service.Save(scenario, (int)index, model);
    }
}
