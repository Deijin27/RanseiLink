using RanseiLink.Core.Services;
using RanseiLink.ViewModels;

namespace RanseiLink.Services;

public delegate IEditorContext EditorContextFactory(IDataService dataService, MainEditorViewModel mainEditorViewModel);

public interface IEditorContext
{
    IDataService DataService { get; }
    IJumpService JumpService { get; }
    ICachedMsgBlockService CachedMsgBlockService { get; }
}
