using CliFx.Infrastructure;
using RanseiLink.Core.Services;

namespace RanseiLink.Console.Services;

public interface ICurrentModService
{
    bool TryGetCurrentMod(IConsole console, out ModInfo mod);
    bool TryGetDataService(IConsole console, out IDataService dataService);
}
