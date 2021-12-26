using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ScenarioPokemonSelectorViewModel ScenarioPokemonSelectorViewModelFactory(IEditorContext context);

public class ScenarioPokemonSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioPokemon, ScenarioPokemonViewModel>
{
    private readonly ScenarioPokemonViewModelFactory _factory;
    private readonly IScenarioPokemonService _service;
    private readonly IEditorContext _context;

    public ScenarioPokemonSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, 0, 199)
    {
        _context = context;
        _factory = container.Resolve<ScenarioPokemonViewModelFactory>();
        _service = context.DataService.ScenarioPokemon;
        Init();
    }

    protected override ScenarioPokemonViewModel NewViewModel(ScenarioId scenarioId, uint id, IScenarioPokemon model) => _factory(model, _context, scenarioId, id);

    protected override IScenarioPokemon RetrieveModel(ScenarioId scenario, uint index)
    {
        return _service.Retrieve(scenario, (int)index);
    }

    protected override void SaveModel(ScenarioId scenario, uint index, IScenarioPokemon model)
    {
        _service.Save(scenario, (int)index, model);
    }
}
