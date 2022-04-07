using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;
using System.IO;
using RanseiLink.Core.RomFs;
using CliFx;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds extract folder", Description = "Extract a copy of a folder and all contents including sub-folders from an nds file system.")]
public class NdsExtractFolderCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath")]
    public string NdsPath { get; set; }

    [CommandOption("folder", 'f', Description = "Path of folder within nds file system. Omitting this will do root folder")]
    public string FilePath { get; set; } = "";

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to extract to; default is a in the same location as the nds file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("ntStartPos", 'n', Description = "Postion in file of name table start offset. Defaults to nds value")]
    public long? NtStartOffsetPosition { get; set; }

    [CommandOption("fatStartPos", 'a', Description = "Postion in file of file allocation table start offset. Defaults to nds value")]
    public long? FatStartOffsetPosition { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrEmpty(DestinationFolder))
        {
            DestinationFolder = Path.GetDirectoryName(NdsPath);
        }
        else
        {
            Directory.CreateDirectory(DestinationFolder);
        }

        var romFsConfig = new RomFsConfig
        (
            NameTableStartOffsetPositon: NtStartOffsetPosition ?? RomFs.NdsConfig.NameTableStartOffsetPositon,
            AllocationTableStartOffsetPosition: FatStartOffsetPosition ?? RomFs.NdsConfig.AllocationTableStartOffsetPosition
        );

        using IRomFs nds = new RomFs(NdsPath, romFsConfig);
        nds.ExtractCopyOfDirectory(FilePath, DestinationFolder);

        return default;
    }
}
