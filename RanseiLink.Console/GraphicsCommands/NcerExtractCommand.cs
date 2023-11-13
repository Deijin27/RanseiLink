using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("ncer extract", Description = "Extract a png from a Nitro Cell Resource")]
public class NcerExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of ncer file", Name = "ncer")]
    public string Ncer { get; set; }

    [CommandParameter(1, Description = "Path of ncgr file", Name = "ncgr")]
    public string Ncgr { get; set; }

    [CommandParameter(2, Description = "Path of nclr file", Name = "nclr")]
    public string Nclr { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination file; default is a file in the same location as the file.")]
    public string DestinationFile { get; set; }

    [CommandOption("width", 'w', Description = "Explicit width of the image. By default this is inferred from the data, because cell images don't technically have a width and height.")]
    public int Width { get; set; } = -1;

    [CommandOption("height", 'e', Description = "Explicit height of the image. By default this is inferred from the data, because cell images don't technically have a width and height.")]
    public int Height { get; set; } = -1;

    [CommandOption("debug", 'b', Description = "Draw debug info on output image")]
    public bool Debug { get; set; } = false;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFile == null)
        {
            DestinationFile = FileUtil.MakeUniquePath(Path.ChangeExtension(Ncgr, "png"));
        }

        var ncer = NCER.Load(Ncer);
        var ncgr = NCGR.Load(Ncgr);
        var nclr = NCLR.Load(Nclr);
        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Width, Height, debug: Debug);
        image.SaveAsPng(DestinationFile);

        return default;
    }
}
