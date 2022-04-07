using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.ViewModels;
using System.Linq;

namespace RanseiLink.Services.Concrete;

internal class JumpService : IJumpService 
{ 
    private readonly MainEditorViewModel _mainEditor;

    public JumpService(MainEditorViewModel mainEditorViewModel)
    {
        _mainEditor = mainEditorViewModel;
    }

    public void JumpTo(string moduleId, int selectId)
    {
        if (!_mainEditor.TryGetModule(moduleId, out var module))
        {
            return;
        }
        if (module.ViewModel is not SelectorViewModel selectorViewModel)
        {
            return;
        }
        selectorViewModel.Selected = selectId;
        _mainEditor.CurrentModuleId = moduleId;
    }

    public void JumpTo(string moduleId)
    {
        if (!_mainEditor.TryGetModule(moduleId, out _))
        {
            return;
        }
        _mainEditor.CurrentModuleId = moduleId;
    }

    public void JumpToScenarioPokemon(ScenarioId scenario, uint id)
    {
        //var moduleId = ScenarioPokemonSelectorEditorModule.Id;
        //if (_mainEditor.ViewModels[moduleId] is ScenarioPokemonSelectorViewModel selectorVm 
        //    && selectorVm.ScenarioItems.Contains(scenario)
        //    && selectorVm.MinIndex <= id
        //    && selectorVm.MaxIndex >= id)
        //{
        //    selectorVm.SelectedScenario = scenario;
        //    selectorVm.SelectedItem = id;
        //    _mainEditor.CurrentModuleId = moduleId;
        //}
    }

    public void JumpToScenarioWarrior(ScenarioId scenario, uint id)
    {
        //var moduleId = ScenarioWarriorSelectorEditorModule.Id;
        //if (_mainEditor.ViewModels[moduleId] is ScenarioWarriorSelectorViewModel selectorVm
        //    && selectorVm.ScenarioItems.Contains(scenario)
        //    && selectorVm.MinIndex <= id
        //    && selectorVm.MaxIndex >= id)
        //{
        //    selectorVm.SelectedScenario = scenario;
        //    selectorVm.SelectedItem = id;
        //    _mainEditor.CurrentModuleId = moduleId;
        //}
    }
}
