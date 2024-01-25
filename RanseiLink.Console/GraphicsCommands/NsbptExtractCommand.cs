using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.Conquest;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtp extract", Description = "Extract human readable xml from nsbtp")]
public class NsbptExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nsbtp data file.", Name = "nsbtpFile")]
    public string FilePath { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    [CommandOption("raw", 'r', Description = "Is the custom minimal format used to store additional animations for pokemon 3d models")]
    public bool Raw { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(FilePath, "xml"));
        }

        XElement el;
        if (Raw)
        {
            el = NSPAT_RAW.Load(FilePath).SerializeRaw();
        }
        else
        {
            el = new NSBTP(FilePath).PatternAnimations.Serialize();
        }

        new XDocument(el).Save(DestinationFile);

        return default;
    }
}
