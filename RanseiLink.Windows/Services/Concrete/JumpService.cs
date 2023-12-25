#nullable enable
using RanseiLink.Windows.ViewModels;

namespace RanseiLink.Windows.Services.Concrete;

internal class JumpService(IMainEditorViewModel mainEditorViewModel) : IJumpService 
{
    public void JumpToMessageFilter(string filter, bool regex = false)
    {
        if (!mainEditorViewModel.TryGetModule(MsgGridEditorModule.Id, out var module))
        {
            return;
        }
        if (module.ViewModel is not MsgGridViewModel vm)
        {
            return;
        }
        vm.UseRegex = false;
        vm.SearchTerm = filter;
        mainEditorViewModel.CurrentModuleId = MsgGridEditorModule.Id;
    }

    public void JumpTo(string moduleId, int selectId)
    {
        if (!mainEditorViewModel.TryGetModule(moduleId, out var module))
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
        mainEditorViewModel.CurrentModuleId = moduleId;
    }

    public void JumpTo(string moduleId)
    {
        if (!mainEditorViewModel.TryGetModule(moduleId, out _))
        {
            return;
        }
        mainEditorViewModel.CurrentModuleId = moduleId;
    }

    public void JumpToNested(string moduleId, int outerSelectedId, int innerSelectedId)
    {
        if (!mainEditorViewModel.TryGetModule(moduleId, out var module))
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

        mainEditorViewModel.CurrentModuleId = moduleId;
    }

}
