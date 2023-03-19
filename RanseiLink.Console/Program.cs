using System.Threading.Tasks;
using CliFx;
using System.Reflection;
using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;

namespace RanseiLink.Console;

internal class Program
{
    public static async Task<int> Main()
    {
        // load services
        var builder = new Container();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new ConsoleServiceModule());

        var modServiceGetter = new ModServiceGetterFactory(builder);
        modServiceGetter.AddModule(new CoreModServiceModule());
        modServiceGetter.AddModule(new ConsoleModServiceModule());
        builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

        // build application with clifx
        return await new CliApplicationBuilder()
           .SetVersion(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion)
           .SetTitle("RanseiLink Console")
           .SetDescription("Pokemon Conquest ROM Editor")
           .AddCommandsFromThisAssembly()
           .UseTypeActivator(t => builder.Resolve(t))
           .Build()
           .RunAsync();
    }
}
