using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Registration;
using RanseiLink.Console.Services.Registration;

namespace RanseiLink.Console;

internal class Program
{
    internal static IServiceContainer ServiceContainer { get; }

    static Program()
    {
        ServiceContainer = new ServiceContainer();
        ServiceContainer.RegisterCoreServices();
        ServiceContainer.RegisterConsoleServices();
    }

    public static async Task<int> Main()
    {
        return await new CliApplicationBuilder()
           .SetVersion("3.0")
           .SetTitle("RanseiLink Console")
           .SetDescription("Pokemon Conquest ROM Editor")
           .AddCommandsFromThisAssembly()
           .Build()
           .RunAsync();
    }
}
