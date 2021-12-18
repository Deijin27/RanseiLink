using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.Tests.Mocks;

internal class MockEditorContext : IEditorContext
{
    public IDataService DataService { get; set; }

    public IJumpService JumpService { get; set; }

    public ICachedMsgBlockService CachedMsgBlockService { get; set; }
}
