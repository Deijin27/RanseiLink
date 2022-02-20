using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("select mod", Description = "Change current mod to specific slot. View slots with 'list mods' command")]
public class SelectModCommand : BaseCommand
{
    public SelectModCommand(IServiceContainer container) : base(container) { }
    public SelectModCommand() : base() { }

    [CommandParameter(0, Description = "Slot to switch to.", Name = "modSlot")]
    public int Slot { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var modService = Container.Resolve<IModService>();
        var settingsService = Container.Resolve<ISettingService>();

        var modInfos = modService.GetAllModInfo();
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
        settingsService.Get<CurrentConsoleModSlotSetting>().Value = Slot;
        settingsService.Save();
        console.Output.WriteLine("Current mod changed to:");
        console.Render(mod);

        return default;
    }
}
