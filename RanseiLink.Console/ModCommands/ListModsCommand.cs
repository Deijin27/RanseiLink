using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("list mods", Description = "View info on all existing mods.")]
public class ListModsCommand : ICommand
{
    private readonly IModManager _modManager;
    private readonly ISettingService _settingService;
    public ListModsCommand(IModManager modManager, ISettingService settingService)
    {
        _modManager = modManager;
        _settingService = settingService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var mods = _modManager.GetAllModInfo();
        if (mods.Count == 0)
        {
            console.Output.WriteLine("No mods found");
        }
        else
        {
            var current = _settingService.Get<CurrentConsoleModSlotSetting>().Value;
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
