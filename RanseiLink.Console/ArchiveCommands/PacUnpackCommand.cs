using RanseiLink.Core.Archive;

namespace RanseiLink.Console.ArchiveCommands;

[Command("pac unpack", Description = "Unpack a PAC archive into its constituent files.")]
public class PacUnpackCommand : ICommand
{
    [CommandParameter(0, Description = "Path of PAC archive to unpack.", Name = "filePath")]
    public string FilePath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        PAC.Unpack(FilePath, DestinationFolder);

        return default;
    }
}
