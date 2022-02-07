using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public static class ImageUtil
{
    public static void SaveAsPngTiled(string file, byte[] pixelMap, Rgba32[] palette, int width, int tileSize)
    {
        SaveAsPng(file, pixelMap, palette, width, (a, b) => PointUtil.GetPointTiled(a, b, tileSize));
    }

    public static void SaveAsPng(string file, byte[] pixelMap, Rgba32[] palette, int width)
    {
        SaveAsPng(file, pixelMap, palette, width, PointUtil.GetPoint);
    }

    private static void SaveAsPng(string file, byte[] pixelArray, Rgba32[] palette, int width, PointGetter pointGetter)
    {
        using var img = ToImage(pixelArray, palette, width, pointGetter);
        img.SaveAsPng(file);
    }

    private static Image<Rgba32> ToImage(byte[] pixelArray, Rgba32[] palette, int width, PointGetter pointGetter)
    {
        int height = pixelArray.Length / width;
        var img = new Image<Rgba32>(width, height);

        for (int i = 0; i < pixelArray.Length; i++)
        {
            Point point = pointGetter(i, width);
            Rgba32 color = palette[pixelArray[i]];
            img[point.X, point.Y] = color;
        }

        return img;
    }

    public static void SaveAsPng(string file, Cell[] bank, uint blockSize, byte[] pixelArray, Rgb15[] pal, int width, int height, bool tiled = false
                                       , bool debug = false)
    {
        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;

        PointGetter pointGetter = tiled ? PointUtil.GetPointTiled8 : PointUtil.GetPoint;

        Rgba32[] palette = RawPalette.To32bitColors(pal);
        palette[0] = Color.Transparent;

        using var graphic = new Image<Rgba32>(width, height);

        for (int i = 0; i < bank.Length; i++)
        {
            Cell cell = bank[i];

            if (cell.Width == 0x00 || cell.Height == 0x00)
                continue;

            int tileOffset = cell.TileOffset << (byte)blockSize;
            int bankDataOffset = 0;
            var startByte = tileOffset * 0x20 + bankDataOffset;
            var endByte = startByte + cell.Width * cell.Height;
            byte[] cellPixels = pixelArray[startByte..endByte];

            using var cellImg = ToImage(cellPixels, palette, cell.Width, pointGetter);

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
                    location: new Point(cell.XOffset, cell.YOffset + yShift),
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
                    g.DrawText(i.ToString(), SystemFonts.CreateFont("Arial", 9), Color.Black, new PointF(cell.XOffset + 2, cell.YOffset + 2 + yShift));
                    g.Draw(new Pen(Color.Red, 1), new RectangleF(cell.XOffset, cell.YOffset + yShift, cell.Width, cell.Height));
                }
            });
        }

        graphic.SaveAsPng(file);
    }

    public static void LoadPng(string file, Cell[] bank, uint blockSize, out byte[] pixelArray, out Rgb15[] pal, out int width, out int height, bool tiled = false)
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
        width = image.Width;
        height = image.Height;

        var palette = new List<Rgba32>
        {
            Color.Transparent
        };
        
        var pixels = new List<byte>();

        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;

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
            byte[] cellPixels = new byte[cell.Width * cell.Height];

            using var cellImg = image.Clone(g =>
            {
                g.Crop(new Rectangle(cell.XOffset, cell.YOffset + yShift, cell.Width, cell.Height));

                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });

            for (int x = 0; x < cell.Width; x++)
            {
                for (int y = 0; y < cell.Height; y++)
                {
                    Rgba32 pixColor = cellImg[x, y];
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
                    cellPixels[indexGetter(new Point(x, y), cell.Width)] = (byte)pltIndex;
                }
            }
            pixels.AddRange(cellPixels);
        }

        pixelArray = pixels.ToArray();
        pal = RawPalette.From32bitColors(palette);
        pal[0] = Rgb15.From(0x3E0);

        image.Dispose();
    }
}
