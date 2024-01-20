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

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _scenarioPokemonService = modServices.Get<IScenarioPokemonService>();
        _scenarioKingdomService = modServices.Get<IScenarioKingdomService>();
        var spVm = modServices.Get<ScenarioPokemonViewModel.Factory>()();
        var vm = modServices.Get<ScenarioWarriorWorkspaceViewModel>().Init(spVm);
        var spService = modServices.Get<IScenarioPokemonService>();

        _viewModel = _selectorVmFactory.Create(_service, vm,
            id => vm.SetModel((ScenarioId)id, _service.Retrieve(id), spService.Retrieve(id)), scrollEnabled: false);
    }

    public override void OnPatchingRom()
    {
        base.OnPatchingRom();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
    }
}
