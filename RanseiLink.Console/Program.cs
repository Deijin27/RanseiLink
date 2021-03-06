using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using Autofac;
using System.Reflection;

namespace RanseiLink.Console;

internal class Program
{
    public static async Task<int> Main()
    {
        // load services
        var builder = new ContainerBuilder();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new ConsoleServiceModule());
        var container = builder.Build();
        ContainerProvider.Container = container;

        // build application with clifx
        return await new CliApplicationBuilder()
           .SetVersion(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion)
           .SetTitle("RanseiLink Console")
           .SetDescription("Pokemon Conquest ROM Editor")
           .AddCommandsFromThisAssembly()
           .UseTypeActivator(t => container.Resolve(t))
           .Build()
           .RunAsync();
    }
}
