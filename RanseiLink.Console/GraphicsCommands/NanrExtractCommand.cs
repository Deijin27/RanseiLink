using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nanr extract", Description = "Print out informational content of Nitro Animation Resource")]
public class NanrExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of G2DR which contains the nscr background", Name = "background")]
    public string Background { get; set; }

    [CommandParameter(1, Description = "Path of G2DR which contains the ncer parts and nanr animation", Name = "anim", IsRequired = false)]
    public string AnimatedParts { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination folder; default is a dir in the same location as the file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("positionRelativeTo", 'p', Description = "Whether cell x and y should be interpreted as relative to centre of background, or top-left of background")]
    public PositionRelativeTo PositionRelativeTo { get; set; } = PositionRelativeTo.TopLeft;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = FileUtil.MakeUniquePath(Path.ChangeExtension(Background, null));
        }
        Directory.CreateDirectory(DestinationFolder);

        CellAnimationSerialiser.Serialise(PositionRelativeTo, DestinationFolder, Background, AnimatedParts);

        return default;
    }
}
