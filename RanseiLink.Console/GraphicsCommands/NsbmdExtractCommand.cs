using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbmd extract", Description = "Extract obj, mtl, and texture pngs from nsbmd")]
public class NsbmdExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder containing nsbmd, nsbtx etc.", Name = "sourceDir")]
    public string SourceDir { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = SourceDir + " - Extracted";
        }
        Directory.CreateDirectory(DestinationFolder);

        ModelExtractorGenerator.ExtractModelFromFolder(SourceDir, DestinationFolder);

        return default;
    }
}
