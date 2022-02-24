using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;

namespace RanseiLink.Tests.Mocks;

class MockCurrentModService : ICurrentModService
{
    public bool TryGetCurrentMod(IConsole console, out ModInfo mod)
    {
        throw new System.NotImplementedException();
    }


    public IModServiceContainer TryGetDataServiceReturn { get; set; }
    public bool TryGetDataServiceSucceed { get; set; }
    public bool TryGetDataService(IConsole console, out IModServiceContainer dataService)
    {
        dataService = TryGetDataServiceReturn;
        return TryGetDataServiceSucceed;
    }
}
