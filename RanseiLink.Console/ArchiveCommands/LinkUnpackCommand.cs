using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Archive;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands
{
    [Command("link unpack", Description = "Unpack a link archive into its constituent files.")]
    public class LinkUnpackCommand : ICommand
    {
        [CommandParameter(0, Description = "Path of link archive to unpack.", Name = "filePath")]
        public string FilePath { get; set; }

        [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
        public string DestinationFolder { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            Link.Unpack(FilePath, DestinationFolder);

            return default;
        }
    }
}
