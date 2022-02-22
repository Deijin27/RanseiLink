using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Archive;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("pac pack", Description = "Pack the contents of a folder into a pac archive.")]
public class PacPackCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder to pack into a link archive.", Name = "filePath")]
    public string FolderPath { get; set; }

    [CommandParameter(1, Description = "For each file in the folder ordered by name, the numbers (format: 0,2,3,1,6) 0 => .bmd0, 1 => .btx0, 2 => .btp0, 4 => .bma0, 5 => .unknown5, 6 => .char, 7 => .bta0", Name = "fileTypeNumbers")]
    public int[] FileTypeNumbers { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional file to pack to; default is a file in the same location as the folder.")]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        PAC.Pack(FolderPath, FileTypeNumbers, DestinationFile);

        return default;
    }
}
