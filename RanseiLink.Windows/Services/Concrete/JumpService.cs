#nullable enable
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
        if (module is ISelectableModule selectableModule)
        {
            selectableModule.Select(selectId);
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
}
