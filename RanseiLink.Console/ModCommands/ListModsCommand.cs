using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("list mods", Description = "View info on all existing mods.")]
public class ListModsCommand : BaseCommand
{
    public ListModsCommand(IServiceContainer container) : base(container) { }
    public ListModsCommand() : base() { }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var modService = Container.Resolve<IModService>();
        var settingService = Container.Resolve<ISettingService>();

        var mods = modService.GetAllModInfo();
        if (mods.Count == 0)
        {
            console.Output.WriteLine("No mods found");
        }
        else
        {
            var current = settingService.Get<CurrentConsoleModSlotSetting>().Value;
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
