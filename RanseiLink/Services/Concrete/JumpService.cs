using RanseiLink.ViewModels;

namespace RanseiLink.Services.Concrete;

internal class JumpService : IJumpService 
{ 
    private readonly IMainEditorViewModel _mainEditor;

    public JumpService(IMainEditorViewModel mainEditorViewModel)
    {
        _mainEditor = mainEditorViewModel;
    }

    public void JumpToMessageFilter(string filter, bool regex = false)
    {
        if (!_mainEditor.TryGetModule(MsgGridEditorModule.Id, out var module))
        {
            return;
        }
        if (module.ViewModel is not MsgGridViewModel vm)
        {
            return;
        }
        vm.UseRegex = false;
        vm.SearchTerm = filter;
        _mainEditor.CurrentModuleId = MsgGridEditorModule.Id;
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
        if (selectorViewModel.Selected != selectId)
        {
            return;
        }
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
        scenarioSelectorViewModel.Selected = outerSelectedId;

        if (scenarioSelectorViewModel.Selected != outerSelectedId)
        {
            return;
        }

        if (scenarioSelectorViewModel.NestedViewModel is SelectorViewModel nestedSelectorViewModel)
        {
            nestedSelectorViewModel.Selected = innerSelectedId;

            if (nestedSelectorViewModel.Selected != innerSelectedId)
            {
                return;
            }
        }

        _mainEditor.CurrentModuleId = moduleId;
    }

}
