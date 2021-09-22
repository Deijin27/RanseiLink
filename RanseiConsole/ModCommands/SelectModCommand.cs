using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("select mod", Description = "Change current mod to specific slot. View slots with 'list mods' command")]
    public class SelectModCommand : ICommand
    {
        [CommandParameter(0, Description = "Slot to switch to.", Name = "modSlot")]
        public int Slot { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            var modInfos = services.CoreServices.ModService.GetAllModInfo();
            if (modInfos.Count == 0)
            {
                console.Output.WriteLine("No mods currently exist");
                return default;
            }
            if (Slot >= modInfos.Count || Slot < 0)
            {
                console.Output.WriteLine("Invalid slot number");
                return default;
            }
            var mod = modInfos[Slot];
            services.CoreServices.Settings.CurrentConsoleModSlot = Slot;
            console.Output.WriteLine("Current mod changed to:");
            console.Render(mod);

            return default;
        }
    }
}
