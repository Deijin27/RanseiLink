using RanseiLink.Core.Services;
using RanseiLink.ViewModels;

namespace RanseiLink.Services;

public delegate IEditorContext EditorContextFactory(IModServiceContainer dataService, MainEditorViewModel mainEditorViewModel);

public interface IEditorContext
{
    IModServiceContainer DataService { get; }
    IJumpService JumpService { get; }
    ICachedMsgBlockService CachedMsgBlockService { get; }
}
