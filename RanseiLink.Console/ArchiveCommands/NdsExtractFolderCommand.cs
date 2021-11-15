using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;
using System.IO;
using RanseiLink.Core.Services;
using RanseiLink.Core.Nds;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds extract folder", Description = "Extract a copy of a folder and all contents including sub-folders from an nds file system.")]
public class NdsExtractFolderCommand : BaseCommand
{
    public NdsExtractFolderCommand(IServiceContainer container) : base(container) { }
    public NdsExtractFolderCommand() : base() { }

    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath")]
    public string NdsPath { get; set; }

    [CommandParameter(1, Description = "Path of folder within nds file system")]
    public string FilePath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to extract to; default is a in the same location as the nds file.")]
    public string DestinationFolder { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var ndsFactory = Container.Resolve<NdsFactory>();

        if (string.IsNullOrEmpty(DestinationFolder))
        {
            DestinationFolder = Path.GetDirectoryName(NdsPath);
        }
        else
        {
            Directory.CreateDirectory(DestinationFolder);
        }

        using INds nds = ndsFactory(NdsPath);
        nds.ExtractCopyOfDirectory(FilePath, DestinationFolder);

        return default;
    }
}
