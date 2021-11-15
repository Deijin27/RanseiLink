using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Registration;
using RanseiLink.Console.Services;

namespace RanseiLink.Console;

internal class Program
{
    public static async Task<int> Main()
    {
        IServiceContainer container = ServiceContainer.Instance;
        container.RegisterCoreServices();
        container.RegisterConsoleServices();

        return await new CliApplicationBuilder()
           .SetVersion("vMinnow-beta1")
           .SetTitle("RanseiLink Console")
           .SetDescription("Pokemon Conquest ROM Editor")
           .AddCommandsFromThisAssembly()
           .Build()
           .RunAsync();
    }
}
