#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;


[EditorModule]
public class ScenarioWarriorWorkspaceEditorModule : BaseSelectorEditorModule<IScenarioWarriorService>
{
    public const string Id = "scenario_warrior_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Warrior";

    private IScenarioPokemonService? _scenarioPokemonService;
    private IScenarioKingdomService? _scenarioKingdomService;
    private IScenarioArmyService? _scenarioArmyService;

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _scenarioPokemonService = modServices.Get<IScenarioPokemonService>();
        _scenarioKingdomService = modServices.Get<IScenarioKingdomService>();
        _scenarioArmyService = modServices.Get<IScenarioArmyService>();
        var spVm = modServices.Get<ScenarioPokemonViewModel.Factory>()();
        var vm = modServices.Get<ScenarioWarriorWorkspaceViewModel>().Init(spVm);

        SelectorViewModel = _selectorVmFactory.Create(_service, vm,
            id => vm.SetModel((ScenarioId)id, 
            _service.Retrieve(id), 
            _scenarioPokemonService.Retrieve(id), 
            _scenarioArmyService.Retrieve(id)
            ), scrollEnabled: false);
    }

    public override void OnPatchingRom()
    {
        base.OnPatchingRom();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
        _scenarioArmyService?.Save();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
        _scenarioArmyService?.Save();
    }
}
