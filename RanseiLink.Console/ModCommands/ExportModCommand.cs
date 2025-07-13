using RanseiLink.Core;
using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("export mod", Description = "Export currently selected mod to destination folder.")]
public class ExportModCommand(ICurrentModService currentModService, IModManager modManager) : ICommand
{
    [CommandParameter(0, Description = "File to export to", Name = "destinationFile", Converter = typeof(PathConverter))]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        string exportedTo = modManager.Export(currentMod, FileUtil.MakeUniquePath(FileUtil.MakeValidFileName(DestinationFile)));

        console.WriteLine($"Mod \"{currentMod.Name}\" exported to \"{exportedTo}\"");
        return default;
    }
}
