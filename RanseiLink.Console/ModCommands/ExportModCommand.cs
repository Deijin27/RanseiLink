using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;

namespace RanseiLink.Console.ModCommands;

[Command("export mod", Description = "Export currently selected mod to destination folder.")]
public class ExportModCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    private readonly IModManager _modManager;
    public ExportModCommand(ICurrentModService currentModService, IModManager modManager)
    {
        _currentModService = currentModService;
        _modManager = modManager;
    }

    [CommandParameter(0, Description = "Folder to export to.", Name = "destinationFolder", Converter = typeof(PathConverter))]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        string exportedTo = _modManager.Export(currentMod, DestinationFolder);

        console.Output.WriteLine($"Mod \"{currentMod.Name}\" exported to \"{exportedTo}\"");
        return default;
    }
}
