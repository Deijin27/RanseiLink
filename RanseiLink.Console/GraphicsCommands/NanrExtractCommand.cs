using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nanr extract", Description = "Print out informational content of Nitro Animation Resource")]
public class NanrExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of G2DR which contains the nscr background", Name = "background")]
    public string Background { get; set; }

    [CommandParameter(1, Description = "Path of G2DR which contains the ncer parts and nanr animation", Name = "anim")]
    public string AnimatedParts { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination folder; default is a dir in the same location as the file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = FileUtil.MakeUniquePath(Path.ChangeExtension(Background, null));
        }
        Directory.CreateDirectory(DestinationFolder);

        CellAnimationSerialiser.Serialise(Background, AnimatedParts, DestinationFolder);

        return default;
    }
}
