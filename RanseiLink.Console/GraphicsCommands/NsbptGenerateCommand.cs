using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.Conquest;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtp generate", Description = "Generate nsbtp from xml")]
public class NsbptGenerateCommand : ICommand
{
    [CommandParameter(0, Description = "Path of xml file.", Name = "xmlFile")]
    public string FilePath { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    [CommandOption("raw", 'r', Description = "Generate the custom minimal format used to store additional animations for pokemon 3d models")]
    public bool Raw { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(FilePath, Raw ? "pat" : "nsbtp"));
        }

        var doc = XDocument.Load(FilePath);
        var root = doc.Root;

        if (Raw)
        {
            if (root.Name != NSPAT_RAW.RootElementName)
            {
                console.Output.WriteLine($"Unexpected root element: {root.Name} (expected: {NSPAT_RAW.RootElementName})");
                return default;
            }
        }
        else
        {
            if (root.Name != NSPAT.RootElementName)
            {
                console.Output.WriteLine($"Unexpected root element: {root.Name} (expected: {NSPAT.RootElementName})");
                return default;
            }

        }

        var nspat = NSPAT.Deserialize(root);

        if (Raw)
        {
            NSPAT_RAW.WriteTo(nspat, DestinationFile);
        }
        else
        {
            var nsbtp = new NSBTP() { PatternAnimations = nspat };
            nsbtp.WriteTo(DestinationFile);
        }

        return default;
    }
}
