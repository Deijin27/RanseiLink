using RanseiLink.Core.Graphics;

namespace RanseiLink.Console.GraphicsCommands;

[Command("simplify palette", Description = "Save a copy of an image that uses fewer colors than the original, while changing it minimally")]
public class SimplifyPaletteCommand : ICommand
{

    [CommandParameter(0, Description = "Path of image file.", Name = "imagePath")]
    public string ImagePath { get; set; }

    [CommandParameter(1, Description = "Maximum number of colors to allow in the image", Name = "maxColors")]
    public int MaxColors { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (ImageSimplifier.SimplifyPalette(ImagePath, MaxColors))
        {
            console.WriteLine("Complete!");
        }
        else
        {
            console.WriteLine("Palette did not require simplification");
        }

        return default;
    }
}