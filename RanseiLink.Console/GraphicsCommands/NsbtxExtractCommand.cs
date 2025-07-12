using RanseiLink.Core.Graphics;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtx extract", Description = "Extract textures from btx0 as pngs")]
public class NsbtxExtractCommand : ICommand
{
    public enum Mode
    {
        OneToOne,
        ManyToOne
    }

    [CommandParameter(0, Description = "Path of nsbtx data file.", Name = "nsbtxFile")]
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

        var btx = new NSBTX(FilePath);

        var textures = btx.Texture.Textures;
        var palettes = btx.Texture.Palettes;

        for (int i = 0; i < textures.Count; i++)
        {
            var tex = textures[i];
            NSTEX.Palette pal;
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
                console.WriteLine($"Texture: {tex.Name} has data:");
                console.WriteLine($"- {nameof(tex.Format)}: {tex.Format}");
                console.WriteLine($"- {nameof(tex.Width)}: {tex.Width}");
                console.WriteLine($"- {nameof(tex.Height)}: {tex.Height}");
                console.WriteLine($"- {nameof(tex.RepeatX)}: {tex.RepeatX}");
                console.WriteLine($"- {nameof(tex.RepeatY)}: {tex.RepeatY}");
                console.WriteLine($"- {nameof(tex.FlipX)}: {tex.FlipX}");
                console.WriteLine($"- {nameof(tex.FlipY)}: {tex.FlipY}");
                console.WriteLine($"- {nameof(tex.Color0Transparent)}: {tex.Color0Transparent}");
            }

            var convPal = new Palette(pal.PaletteData, true);
            ImageUtil.SpriteToPng(Path.Combine(DestinationFolder, tex.Name + ".png"), 
                new SpriteImageInfo(tex.TextureData, convPal, tex.Width, tex.Height,
                    IsTiled: false,
                    Format: tex.Format));
        }

        console.WriteLine("Complete!");

        return default;
    }
}