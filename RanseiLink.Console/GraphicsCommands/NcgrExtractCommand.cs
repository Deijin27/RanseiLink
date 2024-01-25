using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;

namespace RanseiLink.Console.GraphicsCommands;

[Command("ncgr extract", Description = "Extract a png from a Nitro Character Graphic Resource")]
public class NcgrExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of ncgr file", Name = "ncgr")]
    public string Ncgr { get; set; }

    [CommandParameter(1, Description = "Path of nclr file", Name = "nclr")]
    public string Nclr { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(Ncgr, "png"));
        }

        var ncgr = NCGR.Load(Ncgr);
        var nclr = NCLR.Load(Nclr);
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);
        image.SaveAsPng(DestinationFile);

        return default;
    }
}
