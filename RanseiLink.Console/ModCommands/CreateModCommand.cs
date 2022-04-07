using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("create mod", Description = "Create a new mod (and by default set the current mod to it).")]
public class CreateModCommand : ICommand
{
    private readonly IModManager _modManager;
    private readonly ISettingService _settingService;
    public CreateModCommand(IModManager modManager, ISettingService settingService)
    {
        _modManager = modManager;
        _settingService = settingService;
    }

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

    public ValueTask ExecuteAsync(IConsole console)
    {
        ModInfo modInfo = _modManager.Create(RomPath, ModName, ModVersion, ModAuthor);
        if (SetAsCurrent)
        {
            var mods = _modManager.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == modInfo.FolderPath)
                {
                    _settingService.Get<CurrentConsoleModSlotSetting>().Value = i;
                    _settingService.Save();
                    break;
                }
            }
            
        }
        console.Output.WriteLine("Mod created successfully");
        console.Render(modInfo);

        return default;
    }
}
