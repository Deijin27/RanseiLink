#nullable enable
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public class SpriteImageInfo
{
    public SpriteImageInfo(byte[] pixels, Rgba32[] palette, int width, int height)
    {
        Pixels = pixels;
        Palette = palette;
        Width = width;
        Height = height;
    }

    public byte[] Pixels { get; set; } 
    public Rgba32[] Palette { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class InvalidPaletteException : Exception
{
    public InvalidPaletteException(string message) : base(message) { }
}

public static class ImageUtil
{
    public static void SaveAsPng(string file, SpriteImageInfo imageInfo, bool tiled = false, TexFormat format = TexFormat.Pltt16)
    {
        using (var img = ToImage(imageInfo, tiled ? new PointGetter(PointUtil.GetPointTiled8) : new PointGetter(PointUtil.GetPoint), format))
        {
            img.SaveAsPng(file);
        }
    }

    public static Image<Rgba32> ToImage(SpriteImageInfo imageInfo, PointGetter pointGetter, TexFormat format = TexFormat.Pltt16)
    {
        var img = new Image<Rgba32>(imageInfo.Width, imageInfo.Height);

        switch (format)
        {
            case TexFormat.Pltt4:
            case TexFormat.Pltt16:
            case TexFormat.Pltt256:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        color = imageInfo.Palette[pix];

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.A3I5:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        int colorIndex = pix & 0b11111;
                        int alphaSrc = pix >> 5;
                        byte alpha = (byte)((alphaSrc * 4 + alphaSrc / 2) * 8);
                        var baseColor = imageInfo.Palette[colorIndex];
                        color = new Rgba32(baseColor.R, baseColor.G, baseColor.B, alpha);

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.A5I3:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        int colorIndex = pix & 0b111;
                        byte alpha = (byte)((pix >> 3) * 8);
                        var baseColor = imageInfo.Palette[colorIndex];
                        color = new Rgba32(baseColor.R, baseColor.G, baseColor.B, alpha);

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.Direct:
            case TexFormat.None:
            case TexFormat.Comp4x4:
            default:
                throw new Exception($"Invalid tex format {format}");
        }

        return img;
    }

    public static SpriteImageInfo LoadPng(string file, bool tiled = false, TexFormat format = TexFormat.Pltt16, bool color0ToTransparent = true)
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

        int width = image.Width;
        int height = image.Height;
        var palette = new List<Rgba32>();
        if (color0ToTransparent)
        {
            palette.Add(Color.Transparent);
        }

        byte[] pixels;
        try
        {
            pixels = FromImage(image, palette, tiled ? new IndexGetter(PointUtil.GetIndexTiled8) : new IndexGetter(PointUtil.GetIndex), format, color0ToTransparent);
        }
        catch (Exception e)
        {
            throw new Exception($"Error converting image '{file}'", e);
        }
        image.Dispose();

        return new SpriteImageInfo(pixels, palette.ToArray(), width, height);
    }

    public static byte[] FromImage(Image<Rgba32> image, List<Rgba32> palette, IndexGetter indexGetter, TexFormat format = TexFormat.Pltt16, bool color0ToTransparent = true)
    {
        int width = image.Width;
        int height = image.Height;
        byte[] pixels = new byte[width * height];

        switch (format)
        {
            case TexFormat.Pltt4:
            case TexFormat.Pltt16:
            case TexFormat.Pltt256:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            if (color0ToTransparent && pixColor.A != 255)
                            {
                                pltIndex = 0;
                            }
                            else
                            {
                                pixColor.A = 255;
                                pltIndex = palette.IndexOf(pixColor);
                                if (pltIndex == -1)
                                {
                                    pltIndex = palette.Count;
                                    palette.Add(pixColor);
                                }
                                if (pltIndex > byte.MaxValue)
                                {
                                    throw new InvalidPaletteException($"There can not be more than {byte.MaxValue + 1} colors");
                                }
                            }

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    break;
                }
            case TexFormat.A3I5:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            var alphaColor = pixColor.A;
                            pixColor.A = 255;
                            pltIndex = palette.IndexOf(pixColor);
                            if (pltIndex == -1)
                            {
                                pltIndex = palette.Count;
                                palette.Add(pixColor);
                            }
                            //if (pltIndex > 31)
                            //{
                            //    throw new InvalidPaletteException($"There can not be more than 32 colors");
                            //}
                            var alpha = Math.Min(7, alphaColor * 8 / 255); // this accurately reproduces the values that are created the other way, even if it's weird.
                            pltIndex = alpha << 5 | pltIndex;

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    if (palette.Count > 32)
                    {
                        throw new InvalidPaletteException($"There can not be more than 32 colors (there are {palette.Count} colors)");
                    }
                    break;
                }
            case TexFormat.A5I3:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            var alphaColor = pixColor.A;
                            pixColor.A = 255;
                            pltIndex = palette.IndexOf(pixColor);
                            if (pltIndex == -1)
                            {
                                pltIndex = palette.Count;
                                palette.Add(pixColor);
                            }
                            if (pltIndex > 7)
                            {
                                throw new InvalidPaletteException($"There can not be more than 8 colors");
                            }
                            var alpha = alphaColor / 8;
                            pltIndex = alpha << 3 | pltIndex;

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    
                    break;
                }
            case TexFormat.Direct:
            case TexFormat.None:
            case TexFormat.Comp4x4:
                throw new Exception($"Invalid tex format {format}");
        }

        return pixels;
    }

    public static void SaveAsPng(string file, Cell[] bank, uint blockSize, SpriteImageInfo imageInfo, bool tiled = false, bool debug = false)
    {
        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;
        int minX = bank.Min(i => i.XOffset);
        int xShift = minX < 0 ? -minX : 0;

        var width = imageInfo.Width;
        var height = imageInfo.Height;

        if (width < 0 || height < 0)
        {
            int maxY = bank.Max(i => i.YOffset + i.Height);
            int maxX = bank.Max(i => i.XOffset + i.Width);
            width = maxX - minX;
            height = maxY - minY;
        }

        PointGetter pointGetter = tiled ? new PointGetter(PointUtil.GetPointTiled8) : new PointGetter(PointUtil.GetPoint);

        Rgba32[] palette32 = imageInfo.Palette;
        palette32[0] = Color.Transparent;

        using (var graphic = new Image<Rgba32>(width, height))
        {

            for (int i = 0; i < bank.Length; i++)
            {
                Cell cell = bank[i];

                if (cell.Width == 0x00 || cell.Height == 0x00)
                    continue;

                int tileOffset = cell.TileOffset << (byte)blockSize;
                int bankDataOffset = 0;
                var startByte = tileOffset * 0x20 + bankDataOffset;
                byte[] cellPixels = imageInfo.Pixels.Skip(startByte).Take(cell.Width * cell.Height).ToArray();

                using (var cellImg = ToImage(new SpriteImageInfo(cellPixels, palette32, cell.Width, cell.Height), pointGetter))
                {
                    cellImg.Mutate(g =>
                    {
                        if (cell.FlipX)
                            g.Flip(FlipMode.Horizontal);
                        if (cell.FlipY)
                            g.Flip(FlipMode.Vertical);
                    });

                    graphic.Mutate(g =>
                    {
                        g.DrawImage(
                            image: cellImg,
                            location: new Point(cell.XOffset + xShift, cell.YOffset + yShift),
                            opacity: 1);
                    }); 
                }
            }

            if (debug)
            {
                graphic.Mutate(g =>
                {
                    for (int i = 0; i < bank.Length; i++)
                    {
                        var cell = bank[i];
                        g.DrawText(i.ToString(), SystemFonts.CreateFont("Arial", 9), Color.Black, new PointF(cell.XOffset + 2 + xShift, cell.YOffset + 2 + yShift));
                        g.Draw(new Pen(Color.Red, 1), new RectangleF(cell.XOffset + xShift, cell.YOffset + yShift, cell.Width, cell.Height));
                    }
                });
            }

            graphic.SaveAsPng(file); 
        }
    }

    public static SpriteImageInfo LoadPng(string file, Cell[] bank, uint blockSize, bool tiled = false)
    {
        IndexGetter indexGetter = tiled ? new IndexGetter(PointUtil.GetIndexTiled8) : new IndexGetter(PointUtil.GetIndex);

        if (bank.Length == 0)
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        Image<Rgba32> image;
        try
        {
            image = Image.Load<Rgba32>(file);
        }
        catch (UnknownImageFormatException e)
        {
            throw new UnknownImageFormatException(e.Message + $" File='{file}'");
        }
        var width = image.Width;
        var height = image.Height;

        var palette32 = new List<Rgba32>
    {
        Color.Transparent
    };

        var pixels = new List<byte>();

        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;
        int minX = bank.Min(i => i.XOffset);
        int xShift = minX < 0 ? -minX : 0;

        foreach (Cell cell in bank)
        {
            if (cell.Width == 0x00 || cell.Height == 0x00)
            {
                continue;
            }

            int tileOffset = cell.TileOffset << (byte)blockSize;
            int bankDataOffset = 0;
            var startByte = tileOffset * 0x20 + bankDataOffset;
            var endByte = startByte + cell.Width * cell.Height;

            using (var cellImg = image.Clone(g =>
            {
                g.Crop(new Rectangle(cell.XOffset + xShift, cell.YOffset + yShift, cell.Width, cell.Height));

                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            }))
            {
                byte[] cellPixels = FromImage(cellImg, palette32, indexGetter);
                pixels.AddRange(cellPixels);
            }
        }

        var pixelArray = pixels.ToArray();
        palette32[0] = Color.Magenta;

        image.Dispose();

        return new SpriteImageInfo(pixelArray, palette32.ToArray(), width, height);
    }
}