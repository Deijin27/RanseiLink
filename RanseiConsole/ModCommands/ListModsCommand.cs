using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("list mods", Description = "View info on all existing mods.")]
    public class ListModsCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = ConsoleAppServices.Instance;
            var mods = service.CoreServices.ModService.GetAllModInfo();
            if (mods.Count == 0)
            {
                console.Output.WriteLine("No mods found");
            }
            else
            {
                var current = service.CoreServices.Settings.CurrentConsoleModSlot;
                int count = 0;
                foreach (var mod in mods)
                {
                    if (current == count)
                    {
                        console.Render(mod, $"Slot {count++} (CURRENT):");
                    }
                    else
                    {
                        console.Render(mod, $"Slot {count++}:");
                    }
                }
            }
            return default;
        }
    }
}
