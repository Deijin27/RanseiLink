using CliFx.Infrastructure;
using Core.Services;

namespace RanseiConsole.Services
{
    public interface ICurrentModService
    {
        bool TryGetCurrentMod(IConsole console, out ModInfo mod);
        bool TryGetDataService(IConsole console, out IDataService dataService);
    }
}