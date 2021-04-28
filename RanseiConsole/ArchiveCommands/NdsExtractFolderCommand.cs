using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Nds;
using System.Threading.Tasks;
using System.IO;

namespace RanseiConsole.ArchiveCommands
{
    [Command("nds extract folder", Description = "Extract a copy of a folder and all contents including sub-folders from an nds file system.")]
    public class NdsExtractFolderCommand : ICommand
    {
        [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath")]
        public string NdsPath { get; set; }

        [CommandParameter(1, Description = "Path of file within nds file system")]
        public string FilePath { get; set; }

        [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to extract to; default is a in the same location as the nds file.")]
        public string DestinationFolder { get; set; }

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

            using var nds = new Nds(NdsPath);
            nds.ExtractCopyOfDirectory(FilePath, DestinationFolder);

            return default;
        }
    }
}
