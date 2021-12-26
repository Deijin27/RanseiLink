using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ScenarioWarriorSelectorViewModel ScenarioWarriorSelectorViewModelFactory(IEditorContext context);

public class ScenarioWarriorSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioWarrior, ScenarioWarriorViewModel>
{
    private readonly ScenarioWarriorViewModelFactory _factory;
    private readonly IScenarioWarriorService _service;
    private readonly IEditorContext _context;

    public ScenarioWarriorSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, 0, 199)
    {
        _context = context;
        _factory = container.Resolve<ScenarioWarriorViewModelFactory>();
        _service = context.DataService.ScenarioWarrior;
        Init();
    }

    protected override ScenarioWarriorViewModel NewViewModel(ScenarioId scenarioId, uint id, IScenarioWarrior model) => _factory(scenarioId, _context, model);

    protected override IScenarioWarrior RetrieveModel(ScenarioId scenario, uint index)
    {
        return _service.Retrieve(scenario, (int)index);
    }

    protected override void SaveModel(ScenarioId scenario, uint index, IScenarioWarrior model)
    {
        _service.Save(scenario, (int)index, model);
    }
}
