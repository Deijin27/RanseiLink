using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("select mod", Description = "Change current mod to specific slot. View slots with 'list mods' command")]
public class SelectModCommand : ICommand
{
    private readonly IModManager _modManager;
    private readonly ISettingService _settingService;
    public SelectModCommand(IModManager modManager, ISettingService settingService)
    {
        _modManager = modManager;
        _settingService = settingService;
    }

    [CommandParameter(0, Description = "Slot to switch to.", Name = "modSlot")]
    public int Slot { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var modInfos = _modManager.GetAllModInfo();
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
        _settingService.Get<CurrentConsoleModSlotSetting>().Value = Slot;
        _settingService.Save();
        console.Output.WriteLine("Current mod changed to:");
        console.Render(mod);

        return default;
    }
}
