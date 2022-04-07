using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("import mod", Description = "Import mod (and by default sets imported mod to current)")]
public class ImportModCommand : ICommand
{
    private readonly IModManager _modManager;
    private readonly ISettingService _settingService;
    public ImportModCommand(IModManager modManager, ISettingService settingService)
    {
        _modManager = modManager;
        _settingService = settingService;
    }

    [CommandParameter(0, Description = "Path to mod file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after import.")]
    public bool SetAsCurrent { get; set; } = true;

    public ValueTask ExecuteAsync(IConsole console)
    {
        var info = _modManager.Import(Path);
        if (SetAsCurrent)
        {
            var mods = _modManager.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == info.FolderPath)
                {
                    _settingService.Get<CurrentConsoleModSlotSetting>().Value = i;
                    _settingService.Save();
                    break;
                }
            }
        }
        console.Output.WriteLine("Mod imported successfully.");
        console.Render(info);
        return default;
    }
}
