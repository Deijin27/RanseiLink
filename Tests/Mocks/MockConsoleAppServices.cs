using Core.Services;
using RanseiConsole.Services;

namespace Tests.Mocks
{
    internal class MockConsoleAppServices : IConsoleAppServices
    {
        public ICoreAppServices CoreServices { get; set; }

        public ModInfo CurrentMod { get; set; }
    }
}
