using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public record ImageInfo(byte[] Pixels, Rgba32[] Palette, int Width, int Height);

public static class ImageUtil
{
    public static void SaveAsPng(string file, ImageInfo imageInfo, bool tiled = false)
    {
        using var img = ToImage(imageInfo, tiled ? PointUtil.GetPointTiled8 : PointUtil.GetPoint);
        img.SaveAsPng(file);
    }

    private static Image<Rgba32> ToImage(ImageInfo imageInfo, PointGetter pointGetter)
    {
        var img = new Image<Rgba32>(imageInfo.Width, imageInfo.Height);

        for (int i = 0; i < imageInfo.Pixels.Length; i++)
        {
            Point point = pointGetter(i, imageInfo.Width);
            Rgba32 color = imageInfo.Palette[imageInfo.Pixels[i]];
            img[point.X, point.Y] = color;
        }

        return img;
    }

    public static ImageInfo LoadPng(string file, bool tiled = false)
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
        var palette = new List<Rgba32>
        {
            Color.Transparent
        };

        byte[] pixels = FromImage(image, palette, tiled ? PointUtil.GetIndexTiled8 : PointUtil.GetIndex);

        image.Dispose();

        return new ImageInfo(pixels, palette.ToArray(), width, height);
    }

    private static byte[] FromImage(Image<Rgba32> image, List<Rgba32> palette, IndexGetter indexGetter)
    {
        int width = image.Width;
        int height = image.Height;
        byte[] pixels = new byte[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Rgba32 pixColor = image[x, y];
                int pltIndex;
                if (pixColor.A == 0)
                {
                    pltIndex = 0;
                }
                else
                {
                    pltIndex = palette.IndexOf(pixColor);
                    if (pltIndex == -1)
                    {
                        pltIndex = palette.Count;
                        palette.Add(pixColor);
                    }
                    if (pltIndex > byte.MaxValue)
                    {
                        throw new Exception($"There can not be more than {byte.MaxValue + 1} colors");
                    }
                }
                pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
            }
        }

        return pixels;
    }

    public static void SaveAsPng(string file, Cell[] bank, uint blockSize, ImageInfo imageInfo, bool tiled = false, bool debug = false)
    {
        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;
        int minX = bank.Min(i => i.XOffset);
        int xShift = minX < 0 ? -minX : 0;

        PointGetter pointGetter = tiled ? PointUtil.GetPointTiled8 : PointUtil.GetPoint;

        Rgba32[] palette32 = imageInfo.Palette;
        palette32[0] = Color.Transparent;

        using var graphic = new Image<Rgba32>(imageInfo.Width, imageInfo.Height);

        for (int i = 0; i < bank.Length; i++)
        {
            Cell cell = bank[i];

            if (cell.Width == 0x00 || cell.Height == 0x00)
                continue;

            int tileOffset = cell.TileOffset << (byte)blockSize;
            int bankDataOffset = 0;
            var startByte = tileOffset * 0x20 + bankDataOffset;
            var endByte = startByte + cell.Width * cell.Height;
            byte[] cellPixels = imageInfo.Pixels[startByte..endByte];

            using var cellImg = ToImage(new ImageInfo(cellPixels, palette32, cell.Width, cell.Height), pointGetter);

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

    public static ImageInfo LoadPng(string file, Cell[] bank, uint blockSize, bool tiled = false)
    {
        IndexGetter indexGetter = tiled ? PointUtil.GetIndexTiled8 : PointUtil.GetIndex;

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

            using var cellImg = image.Clone(g =>
            {
                g.Crop(new Rectangle(cell.XOffset + xShift, cell.YOffset + yShift, cell.Width, cell.Height));

                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });

            byte[] cellPixels = FromImage(cellImg, palette32, indexGetter);
            pixels.AddRange(cellPixels);
        }

        var pixelArray = pixels.ToArray();
        palette32[0] = Color.Magenta;

        image.Dispose();

        return new ImageInfo(pixelArray, palette32.ToArray(), width, height);
    }
}
