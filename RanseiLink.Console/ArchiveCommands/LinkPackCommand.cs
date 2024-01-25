using RanseiLink.Core.Archive;

namespace RanseiLink.Console.ArchiveCommands;

[Command("link pack", Description = "Pack the contents of a folder into a link archive.")]
public class LinkPackCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder to pack into a link archive.", Name = "filePath")]
    public string FolderPath { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional file to pack to; default is a file in the same location as the folder.")]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        LINK.Pack(FolderPath, DestinationFile);

        return default;
    }
}
