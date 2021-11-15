using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("export mod", Description = "Export currently selected mod to destination folder.")]
public class ExportModCommand : BaseCommand
{
    public ExportModCommand(IServiceContainer container) : base(container) { }
    public ExportModCommand() : base() { }

    [CommandParameter(0, Description = "Folder to export to.", Name = "destinationFolder", Converter = typeof(PathConverter))]
    public string DestinationFolder { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        var modService = Container.Resolve<IModService>();

        if (!currentModService.TryGetCurrentMod(console, out ModInfo currentMod))
        {
            return default;
        }

        string exportedTo = modService.Export(currentMod, DestinationFolder);

        console.Output.WriteLine($"Mod \"{currentMod.Name}\" exported to \"{exportedTo}\"");
        return default;
    }
}
