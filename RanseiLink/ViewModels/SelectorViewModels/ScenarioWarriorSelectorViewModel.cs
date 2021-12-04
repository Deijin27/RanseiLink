using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate ScenarioWarriorSelectorViewModel ScenarioWarriorSelectorViewModelFactory(IDataService service);

public class ScenarioWarriorSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioWarrior, ScenarioWarriorViewModel>
{
    private readonly ScenarioWarriorViewModelFactory _factory;
    private readonly IScenarioWarriorService _service;
    private readonly IDataService _dataService;

    public ScenarioWarriorSelectorViewModel(IServiceContainer container, IDataService contextualDataService)
        : base(container, 0, 199)
    {
        _factory = container.Resolve<ScenarioWarriorViewModelFactory>();
        _service = contextualDataService.ScenarioWarrior;
        _dataService = contextualDataService;
        Init();
    }

    protected override ScenarioWarriorViewModel NewViewModel(ScenarioId scenarioId, IScenarioWarrior model) => _factory(scenarioId, _dataService, model);

    protected override IScenarioWarrior RetrieveModel(ScenarioId scenario, uint index)
    {
        return _service.Retrieve(scenario, (int)index);
    }

    protected override void SaveModel(ScenarioId scenario, uint index, IScenarioWarrior model)
    {
        _service.Save(scenario, (int)index, model);
    }
}
