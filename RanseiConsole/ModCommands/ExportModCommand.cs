using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("export mod", Description = "Export currently selected mod to destination folder.")]
    public class ExportModCommand : ICommand
    {
        [CommandParameter(0, Description = "Folder to export to.", Name = "destinationFolder", Converter = typeof(PathConverter))]
        public string DestinationFolder { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IConsoleAppServices services = ConsoleAppServices.Instance;
            IModService modManager = services.CoreServices.ModService;
            var currentMod = services.CurrentMod;
            string exportedTo = modManager.Export(currentMod, DestinationFolder);
           
            if (currentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            console.Output.WriteLine($"Mod \"{currentMod.Name}\" exported to \"{exportedTo}\"");
            return default;
        }
    }
}
