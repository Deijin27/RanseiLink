using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("current mod", Description = "Current Mod.")]
    public class CurrentModCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = ConsoleAppServices.Instance;
            var mod = service.CurrentMod;
            if (mod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            console.Render(service.CurrentMod);
            return default;
        }
    }
}
