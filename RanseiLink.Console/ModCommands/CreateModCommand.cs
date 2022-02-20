using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("create mod", Description = "Create a new mod (and by default set the current mod to it).")]
public class CreateModCommand : BaseCommand
{
    public CreateModCommand(IServiceContainer container) : base(container) { }
    public CreateModCommand() : base() { }

    [CommandParameter(0, Description = "Path to unchanged rom file to serve as a base.", Name = "romPath", Converter = typeof(PathConverter))]
    public string RomPath { get; set; }

    [CommandOption("name", 'n', Description = "Name of mod")]
    public string ModName { get; set; }

    [CommandOption("version", 'v', Description = "Version of mod")]
    public string ModVersion { get; set; }

    [CommandOption("author", 'a', Description = "Author of mod")]
    public string ModAuthor { get; set; }

    [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after creation.")]
    public bool SetAsCurrent { get; set; } = true;

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var modService = Container.Resolve<IModService>();
        
        ModInfo modInfo = modService.Create(RomPath, ModName, ModVersion, ModAuthor);
        if (SetAsCurrent)
        {
            var mods = modService.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == modInfo.FolderPath)
                {
                    var settingService = Container.Resolve<ISettingService>();
                    settingService.Get<CurrentConsoleModSlotSetting>().Value = i;
                    settingService.Save();
                    break;
                }
            }
            
        }
        console.Output.WriteLine("Mod created successfully");
        console.Render(modInfo);

        return default;
    }
}
