using System.Threading.Tasks;
using CliFx;

namespace RanseiConsole
{
    class Program
    {
        public static async Task<int> Main() =>
           await new CliApplicationBuilder()
               .SetVersion("v0.1.0")
               .SetTitle("RanseiLink Console")
               .SetDescription("Pokemon Conquest ROM Editor")
               .AddCommandsFromThisAssembly()
               .Build()
               .RunAsync();
    }
}
