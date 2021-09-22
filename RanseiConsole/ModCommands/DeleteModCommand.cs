using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("delete mod", Description = "Delete current mod.")]
    public class DeleteModCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = ConsoleAppServices.Instance;
            var currentMod = service.CurrentMod;
            if (currentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            service.CoreServices.ModService.Delete(currentMod);
            console.Output.WriteLine("Current mod deleted. Info of deleted mod:");
            console.Render(currentMod);
            return default;
        }
    }
}
