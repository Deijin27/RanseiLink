using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("export mod", Description = "Export currently selected mod to destination folder.")]
public class ExportModCommand(ICurrentModService currentModService, IModManager modManager) : ICommand
{
    [CommandParameter(0, Description = "Folder to export to.", Name = "destinationFolder", Converter = typeof(PathConverter))]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        string exportedTo = modManager.Export(currentMod, DestinationFolder);

        console.Output.WriteLine($"Mod \"{currentMod.Name}\" exported to \"{exportedTo}\"");
        return default;
    }
}
