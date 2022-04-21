using RanseiLink.ViewModels;

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

    public void JumpToNested(string moduleId, int outerSelectedId, int innerSelectedId)
    {
        if (!_mainEditor.TryGetModule(moduleId, out var module))
        {
            return;
        }
        if (module.ViewModel is not SelectorViewModel scenarioSelectorViewModel)
        {
            return;
        }
        scenarioSelectorViewModel.Selected = (int)outerSelectedId;

        if (scenarioSelectorViewModel.NestedViewModel is SelectorViewModel nestedSelectorViewModel)
        {
            nestedSelectorViewModel.Selected = innerSelectedId;
        }

        _mainEditor.CurrentModuleId = moduleId;
    }

}
