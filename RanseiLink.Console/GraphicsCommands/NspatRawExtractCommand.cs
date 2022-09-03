using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Graphics.Conquest;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nspatraw extract", Description = "Extract human readable xml from NSPAT_RAW")]
public class NspatRawExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nspatraw data file.", Name = "nspatrawFile")]
    public string FilePath { get; set; }


    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(FilePath, "xml"));
        }

        var el = NSPAT_RAW.Load(FilePath).Serialize();
        new XDocument(el).Save(DestinationFile);


        return default;
    }
}
