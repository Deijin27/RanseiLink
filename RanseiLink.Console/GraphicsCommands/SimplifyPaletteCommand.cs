using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("simplify palette", Description = "Save a copy of an image that uses fewer colors than the original, while changing it minimally")]
public class SimplifyPaletteCommand : BaseCommand
{
    public SimplifyPaletteCommand(IServiceContainer container) : base(container) { }
    public SimplifyPaletteCommand() : base() { }

    [CommandParameter(0, Description = "Path of image file.", Name = "imagePath")]
    public string ImagePath { get; set; }

    [CommandParameter(1, Description = "Maximum number of colors to allow in the image", Name = "maxColors")]
    public int MaxColors { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        if (PaletteSimplifier.SimplifyPalette(ImagePath, MaxColors))
        {
            console.Output.WriteLine("Complete!");
        }
        else
        {
            console.Output.WriteLine("Palette did not require simplification");
        }

        return default;
    }
}