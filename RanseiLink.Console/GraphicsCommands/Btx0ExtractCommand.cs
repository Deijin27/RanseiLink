using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("btx0 extract", Description = "Extract textures from btx0 as pngs")]
public class Btx0ExtractCommand : ICommand
{
    public enum Mode
    {
        OneToOne,
        ManyToOne
    }

    [CommandParameter(0, Description = "Path of btx0 data file.", Name = "btx0File")]
    public string FilePath { get; set; }

   
    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("mode", 'm')]
    public Mode ChosenMode { get; set; } = Mode.OneToOne;

    [CommandOption("debug", Description = "Log debug info")]
    public bool LogDebug { get; set; } = false;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + " - Extracted");
        }
        Directory.CreateDirectory(DestinationFolder);

        var btx = new BTX0(FilePath);

        var textures = btx.Texture.Textures;
        var palettes = btx.Texture.Palettes;

        for (int i = 0; i < textures.Count; i++)
        {
            var tex = textures[i];
            TEX0.Palette pal;
            if (ChosenMode == Mode.ManyToOne)
            {
                pal = palettes[0];
            }
            else // Mode.OneToOne
            {
                pal = palettes[i];
            }
            
            if (LogDebug)
            {
                console.Output.WriteLine($"Texture: {tex.Name} has data:");
                console.Output.WriteLine($"- {nameof(tex.Format)}: {tex.Format}");
                console.Output.WriteLine($"- {nameof(tex.Width)}: {tex.Width}");
                console.Output.WriteLine($"- {nameof(tex.Height)}: {tex.Height}");
                console.Output.WriteLine($"- {nameof(tex.RepeatX)}: {tex.RepeatX}");
                console.Output.WriteLine($"- {nameof(tex.RepeatY)}: {tex.RepeatY}");
                console.Output.WriteLine($"- {nameof(tex.FlipX)}: {tex.FlipX}");
                console.Output.WriteLine($"- {nameof(tex.FlipY)}: {tex.FlipY}");
                console.Output.WriteLine($"- {nameof(tex.Color0Transparent)}: {tex.Color0Transparent}");
            }

            var convPal = RawPalette.To32bitColors(pal.PaletteData);
            if (tex.Color0Transparent)
            {
                convPal[0] = SixLabors.ImageSharp.Color.Transparent;
            }
            ImageUtil.SaveAsPng(Path.Combine(DestinationFolder, tex.Name + ".png"), 
                new ImageInfo(tex.TextureData, convPal, tex.Width, tex.Height),
                tiled: false,
                format: tex.Format);
        }

        console.Output.WriteLine("Complete!");

        return default;
    }
}