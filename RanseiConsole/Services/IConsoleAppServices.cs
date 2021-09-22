using Core.Services;

namespace RanseiConsole.Services
{
    public interface IConsoleAppServices
    {
        ICoreAppServices CoreServices { get; }
        ModInfo CurrentMod { get; }
    }
}