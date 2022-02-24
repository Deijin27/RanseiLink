using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds extract file", Description = "Extract a copy of a file from an nds file system.")]
public class NdsExtractFileCommand : BaseCommand
{
    public NdsExtractFileCommand(IServiceContainer container) : base(container) { }
    public NdsExtractFileCommand() : base() { }

    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath", Converter = typeof(PathConverter))]
    public string NdsPath { get; set; }

    [CommandParameter(1, Description = "Path of file within nds file system", Converter = typeof(PathConverter))]
    public string FilePath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to extract to; default is a in the same location as the nds file.", Converter = typeof(PathConverter))]
    public string DestinationFolder { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var ndsFactory = Container.Resolve<RomFsFactory>();

        if (string.IsNullOrEmpty(DestinationFolder))
        {
            DestinationFolder = Path.GetDirectoryName(NdsPath);
        }
        else
        {
            Directory.CreateDirectory(DestinationFolder);
        }

        using IRomFs nds = ndsFactory(NdsPath);
        nds.ExtractCopyOfFile(FilePath, DestinationFolder);

        return default;
    }
}
