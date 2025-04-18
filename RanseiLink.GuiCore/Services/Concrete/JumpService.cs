namespace RanseiLink.GuiCore.Services.Concrete;

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
        mainEditorViewModel.NavigateTo(MsgGridEditorModule.Id);
    }

    public void JumpTo(string moduleId, int selectId)
    {
        mainEditorViewModel.NavigateTo(moduleId, selectId);
    }

    public void JumpTo(string moduleId)
    {
        if (!mainEditorViewModel.TryGetModule(moduleId, out _))
        {
            return;
        }
        mainEditorViewModel.NavigateTo(moduleId);
    }
}
