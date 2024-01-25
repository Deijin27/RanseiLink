using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.ModCommands;

[Command("select mod", Description = "Change current mod to specific slot. View slots with 'list mods' command")]
public class SelectModCommand(IModManager modManager, ISettingService settingService) : ICommand
{
    [CommandParameter(0, Description = "Slot to switch to.", Name = "modSlot")]
    public int Slot { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var modInfos = modManager.GetAllModInfo();
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
        settingService.Get<CurrentConsoleModSlotSetting>().Value = Slot;
        settingService.Save();
        console.Output.WriteLine("Current mod changed to:");
        console.Render(mod);

        return default;
    }
}
