using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("scbg unpack", Description = "Unpack an SCBG collection into it's constituent images")]
public class ScbgUnpackCommand : BaseCommand
{
    public ScbgUnpackCommand(IServiceContainer container) : base(container) { }
    public ScbgUnpackCommand() : base() { }

    [CommandParameter(0, Description = "Path of scbg data file.", Name = "scbgDataFile")]
    public string FilePath { get; set; }

    [CommandParameter(1, Description = "Path of scbg info file.", Name = "scbgInfoFile")]
    public string InfoFile { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("tiled", 't', Description = "Whether the images are tiled")]
    public bool Tiled { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + " - Unpacked");
        }
        Directory.CreateDirectory(DestinationFolder);

        using var br = new BinaryReader(File.OpenRead(FilePath));

        SCBGCollection
            .Load(FilePath, InfoFile)
            .SaveAsPngs(DestinationFolder, tiled:Tiled);
        
        console.Output.WriteLine("Complete!");

        return default;
    }
}