using System.Threading.Tasks;
using CliFx;
using Core.Services;
using Core.Services.Registration;
using RanseiConsole.Services;

namespace RanseiConsole
{
    internal class Program
    {
        public static async Task<int> Main()
        {
            IServiceContainer container = ServiceContainer.Instance;
            container.RegisterCoreServices();
            container.RegisterConsoleServices();

            return await new CliApplicationBuilder()
               .SetVersion("v0.2.0")
               .SetTitle("RanseiLink Console")
               .SetDescription("Pokemon Conquest ROM Editor")
               .AddCommandsFromThisAssembly()
               .Build()
               .RunAsync();
        }
    }
}
