
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public interface IEditorModule
{
    public string UniqueId { get; }
    public string ListName { get; }
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context);
}
