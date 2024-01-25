using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.ModCommands;

[Command("list mods", Description = "View info on all existing mods.")]
public class ListModsCommand(IModManager modManager, ISettingService settingService) : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        var mods = modManager.GetAllModInfo();
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
