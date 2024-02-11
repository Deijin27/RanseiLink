using RanseiLink.Core;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nanr extract", Description = "Print out informational content of Nitro Animation Resource")]
public class NanrExtractCommand : ICommand
{
    [CommandOption("background", 'b', Description = "Path of G2DR which contains the nscr background")]
    public string Background { get; set; }

    [CommandOption("animation", 'a', Description = "Path of G2DR which contains the ncer parts and nanr animation")]
    public string AnimatedParts { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination folder; default is a dir in the same location as the file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("positionRelativeTo", 'p', Description = "Whether cell x and y should be interpreted as relative to centre of background, or top-left of background")]
    public PositionRelativeTo PositionRelativeTo { get; set; } = PositionRelativeTo.TopLeft;

    [CommandOption("format", 'f', Description = "Serialisation format")]
    public RLAnimationFormat Format { get; set; } = RLAnimationFormat.OneImagePerBank;

    [CommandOption("width", 'w', Description = "If no background is specified, the width the images should be")]
    public int Width { get; set; }

    [CommandOption("height", 'e', Description = "If no background is specified, the height the images hsould be ")]
    public int Height { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = FileUtil.MakeUniquePath(Path.ChangeExtension(Background ?? AnimatedParts, null));
        }
        Directory.CreateDirectory(DestinationFolder);

        var settings = new CellImageSettings(Prt: PositionRelativeTo);

        if (Background != null)
        {
            CellAnimationSerialiser.Export(settings, Format, DestinationFolder, Background, AnimatedParts);
        }
        else if (AnimatedParts != null)
        {
            if (Width <= 0 || Height <= 0)
            {
                throw new System.Exception("Width and height must be specified if there is no background");
            }
            CellAnimationSerialiser.ExportAnimationOnly(settings, DestinationFolder, AnimatedParts, Width, Height, Format, null);
        }

        return default;
    }
}
