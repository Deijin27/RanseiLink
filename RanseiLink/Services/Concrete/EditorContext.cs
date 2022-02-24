using RanseiLink.Core.Services;
using RanseiLink.ViewModels;

namespace RanseiLink.Services.Concrete;

internal class EditorContext : IEditorContext
{
    public EditorContext(IModServiceContainer dataService, MainEditorViewModel mainEditorViewModel)
    {
        DataService = dataService;
        JumpService = new JumpService(mainEditorViewModel);
        CachedMsgBlockService = new CachedMsgBlockService(dataService.Msg);
    }

    public IModServiceContainer DataService { get; }

    public IJumpService JumpService { get; }

    public ICachedMsgBlockService CachedMsgBlockService { get; }
}
