using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtx generate", Description = "Generate new nsbtx from textures")]
public class NsbtxGenerateCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder to pack into a nsbtx.", Name = "filePath")]
    public string FolderPath { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional file to pack to; default is a file in the same location as the folder.")]
    public string DestinationFile { get; set; }

    [CommandOption("transparencyFormat", 't', Description = "Texture format to use for images with transparency")]
    public TexFormat TransparencyFormat { get; set; } = TexFormat.Pltt256;

    [CommandOption("opacityFormat", 'o', Description = "Texture format to use for images without transparency")]
    public TexFormat OpacityFormat { get; set; } = TexFormat.Pltt256;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrEmpty(DestinationFile))
        {
            DestinationFile = Path.Combine(Path.GetDirectoryName(FolderPath), Path.GetFileNameWithoutExtension(FolderPath) + ".nsbtx");
        }

        var files = Directory.GetFiles(FolderPath);
        Array.Sort(files);

        var tex0 = new NSTEX();
        foreach ( var file in files)
        {
            var (texture, palette) = LoadTextureFromImage(file, TransparencyFormat, OpacityFormat);
            tex0.Textures.Add(texture);
            tex0.Palettes.Add(palette);
        }
        var btx0 = new NSBTX { Texture = tex0 };
        btx0.WriteTo(DestinationFile);

        return default;
    }

    private static bool ImageHasTransparency(Image<Rgba32> image)
    {
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                if (image[x, y].A != 255)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static (NSTEX.Texture texture, NSTEX.Palette palette) LoadTextureFromImage(string file, TexFormat transparencyFormat, TexFormat opacityFormat)
    {
        
        Image<Rgba32> image;
        try
        {
            image = Image.Load<Rgba32>(file);
        }
        catch (UnknownImageFormatException e)
        {
            throw new UnknownImageFormatException(e.Message + $" File='{file}'");
        }

        bool imageHasTransparency = ImageHasTransparency(image);
        bool colorZeroTransparent = imageHasTransparency && (transparencyFormat == TexFormat.Pltt4 || transparencyFormat == TexFormat.Pltt16 || transparencyFormat == TexFormat.Pltt256);
        var format = imageHasTransparency ? transparencyFormat : opacityFormat;

        int width = image.Width;
        int height = image.Height;
        var palette = new List<Rgba32>();
        if (colorZeroTransparent)
        {
            palette.Add(Color.Transparent);
        }

        byte[] pixels;
        try
        {
            pixels = ImageUtil.FromImage(image, palette, PointUtil.GetIndex, format, colorZeroTransparent);
        }
        catch (Exception e)
        {
            throw new Exception($"Error converting image '{file}'", e);
        }
        image.Dispose();

        var imgInfo = new ImageInfo(pixels, palette.ToArray(), width, height);

        string texName = Path.GetFileNameWithoutExtension(file);
        if (texName.Length > 13) // 14 to allow the palette to add the _pl
        {
            throw new Exception("Texture name is too long. Max length is 13");
        }
        var texResult = new NSTEX.Texture
        {
            Name = texName,
            TextureData = pixels,
            Color0Transparent = colorZeroTransparent,
            Format = format,
            Width = width,
            Height = height,
        };
        var outPal = RawPalette.From32bitColors(palette);
        Array.Resize(ref outPal, format.PaletteSize());
        var palResult = new NSTEX.Palette { Name = texName + "_pl", PaletteData =  outPal };

        return (texResult, palResult);
    }
}
