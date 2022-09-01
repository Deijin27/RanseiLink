using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtp extract", Description = "Extract human readable xml from nsbtp")]
public class NsbptExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nsbtp data file.", Name = "nsbtpFile")]
    public string FilePath { get; set; }


    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(FilePath, "xml"));
        }

        ModelExtractorGenerator.ExtractPatternAnim(FilePath, DestinationFile);


        return default;
    }
}
