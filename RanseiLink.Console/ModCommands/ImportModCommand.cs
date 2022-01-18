using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("import mod", Description = "Import mod (and by default sets imported mod to current)")]
public class ImportModCommand : BaseCommand
{
    public ImportModCommand(IServiceContainer container) : base(container) { }
    public ImportModCommand() : base() { }

    [CommandParameter(0, Description = "Path to mod file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after import.")]
    public bool SetAsCurrent { get; set; } = true;

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var modService = Container.Resolve<IModService>();
        var settingsService = Container.Resolve<ISettingsService>();

        var info = modService.Import(Path);
        if (SetAsCurrent)
        {
            var mods = modService.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == info.FolderPath)
                {
                    settingsService.CurrentConsoleModSlot = i;
                    break;
                }
            }
        }
        console.Output.WriteLine("Mod imported successfully.");
        console.Render(info);
        return default;
    }
}
