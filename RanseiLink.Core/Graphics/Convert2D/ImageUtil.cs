using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public class InvalidPaletteException : Exception
{
    public InvalidPaletteException(string message) : base(message) { }
}

public static class ImageUtil
{
    public static void SaveAsPng(string file, SpriteImageInfo imageInfo, bool tiled = false, TexFormat format = TexFormat.Pltt16)
    {
        using (var img = ToImage(imageInfo, tiled, format))
        {
            img.SaveAsPng(file);
        }
    }

    public static Image<Rgba32> ToImage(SpriteImageInfo imageInfo, bool tiled = false, TexFormat format = TexFormat.Pltt16)
    {
        var pointGetter = PointUtil.DecidePointGetter(tiled);
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
            pixels = FromImage(image, palette, tiled, format, color0ToTransparent);
        }
        catch (Exception e)
        {
            throw new Exception($"Error converting image '{file}'", e);
        }
        image.Dispose();

        return new SpriteImageInfo(pixels, palette.ToArray(), width, height);
    }

    public static byte[] FromImage(Image<Rgba32> image, List<Rgba32> palette, bool tiled, TexFormat format = TexFormat.Pltt16, bool color0ToTransparent = true)
    {
        var indexGetter = PointUtil.DecideIndexGetter(tiled);
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

    private static BankDimensions InferDimensions(Cell[] bank, int width = -1, int height = -1)
    {
        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;
        int minX = bank.Min(i => i.XOffset);
        int xShift = minX < 0 ? -minX : 0;

        if (width < 0 || height < 0)
        {
            int maxY = bank.Max(i => i.YOffset + i.Height);
            int maxX = bank.Max(i => i.XOffset + i.Width);
            width = maxX - minX;
            height = maxY - minY;
        }

        return new BankDimensions(minX, minY, xShift, yShift, width, height);
    }

    private record BankDimensions(int MinX, int MinY, int XShift, int YShift, int Width, int Height);

    public static Image<Rgba32> ToImage(Cell[] bank, uint blockSize, SpriteImageInfo imageInfo, bool tiled = false, bool debug = false, TexFormat format = TexFormat.Pltt256)
    {
        var dims = InferDimensions(bank, imageInfo.Width, imageInfo.Height);

        Rgba32[] palette32 = imageInfo.Palette;
        palette32[0] = Color.Transparent;

        var graphic = new Image<Rgba32>(dims.Width, dims.Height);
        for (int i = 0; i < bank.Length; i++)
        {
            Cell cell = bank[i];

            if (cell.Width == 0x00 || cell.Height == 0x00)
                continue;

            int tileOffset = cell.TileOffset << (byte)blockSize;
            int bankDataOffset = 0;
            var startByte = tileOffset * 0x20 + bankDataOffset;
            if (format == TexFormat.Pltt16)
            {
                startByte *= 2; // account for compression e.g. pokemon conquest minimaps
            }
            byte[] cellPixels = imageInfo.Pixels.Skip(startByte).Take(cell.Width * cell.Height).ToArray();

            using (var cellImg = ToImage(new SpriteImageInfo(cellPixels, palette32, cell.Width, cell.Height), tiled, format))
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
                        location: new Point(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift),
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

                    // These two operations don't work until
                    // imagesharp.drawing releases version
                    // compatible with v3
                    // but I needed to update for a new feature
                    // to use in 3d model viewer
                    // so these are out of comission for now
                    g.DrawText(i.ToString(), SystemFonts.CreateFont("Arial", 9), Color.Black, new PointF(cell.XOffset + 2 + dims.XShift, cell.YOffset + 2 + dims.YShift));
                    g.Draw(new Pen(Color.Red, 1), new RectangleF(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift, cell.Width, cell.Height));
                }
            });
        }

        return graphic;
    }

    public static void SaveAsPng(string file, Cell[] bank, uint blockSize, SpriteImageInfo imageInfo, bool tiled = false, bool debug = false, TexFormat format = TexFormat.Pltt256)
    {
        using var graphic = ToImage(bank, blockSize, imageInfo, tiled, debug, format);
        graphic.SaveAsPng(file);
    }

    public static Image<Rgba32> ToImage(IList<Cell[]> banks, uint blockSize, SpriteImageInfo imageInfo, bool tiled = false, bool debug = false, TexFormat format = TexFormat.Pltt256)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }
        else if (banks.Count == 1)
        {
            return ToImage(banks[0], blockSize, imageInfo, tiled, debug, format);
        }
        else
        {
            var images = banks.Select(bank => ToImage(bank, blockSize, imageInfo, tiled, debug, format)).ToList();

            var fullHeight = images.Sum(x => x.Height);
            var fullWidth = images.Max(x => x.Width);

            var fullImage = new Image<Rgba32>(fullWidth, fullHeight);
            var cumulativeHeight = 0;
            foreach (var subImage in images)
            {
                fullImage.Mutate(g =>
                {
                    g.DrawImage(subImage, new Point(0, cumulativeHeight), 1);
                });
                cumulativeHeight += subImage.Height;
            }
            return fullImage;
        }
    }

    private static void FromImage(List<byte> workingPixels, List<Rgba32> workingPalette, Image<Rgba32> image, Cell[] bank, uint blockSize, bool tiled, TexFormat format = TexFormat.Pltt256)
    {
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
            if (format == TexFormat.Pltt16)
            {
                startByte *= 2; // account for compression e.g. pokemon conquest minimaps
            }

            using (var cellImg = image.Clone(g =>
            {
                g.Crop(new Rectangle(cell.XOffset + xShift, cell.YOffset + yShift, cell.Width, cell.Height));

                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            }))
            {
                byte[] cellPixels = FromImage(cellImg, workingPalette, tiled);
                workingPixels.AddRange(cellPixels);
            }
        }
    }

    public static Image<Rgba32> LoadPng(string file)
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
        return image;
    }

    public static SpriteImageInfo LoadPng(string file, Cell[] bank, uint blockSize, bool tiled = false, TexFormat format = TexFormat.Pltt256)
    {
        if (bank.Length == 0)
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = LoadPng(file);

        return FromImage(image, bank, blockSize, tiled, format);
    }

    public static SpriteImageInfo FromImage(Image<Rgba32> image, Cell[] bank, uint blockSize, bool tiled = false, TexFormat format = TexFormat.Pltt256)
    {
        var width = image.Width;
        var height = image.Height;

        var workingPalette = new List<Rgba32>
        {
            Color.Transparent
        };

        var workingPixels = new List<byte>();

        FromImage(workingPixels, workingPalette, image, bank, blockSize, tiled, format);

        var pixelArray = workingPixels.ToArray();
        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(pixelArray, workingPalette.ToArray(), width, height);
    }

    public static SpriteImageInfo LoadMutiBankCellSpriteFromPng(string file, IList<Cell[]> banks, uint blockSize, bool tiled = false, TexFormat format = TexFormat.Pltt256)
    {
        if (banks.Any(x => x.Length == 0))
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = LoadPng(file);

        return FromImage(image, banks, blockSize, tiled, format);
    }

    public static SpriteImageInfo FromImage(Image<Rgba32> image, IList<Cell[]> banks, uint blockSize, bool tiled = false, TexFormat format = TexFormat.Pltt256)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }
        if (banks.Count == 1)
        {
            return FromImage(image, banks[0], blockSize, tiled, format);
        }

        var width = image.Width;
        var height = image.Height;

        var workingPalette = new List<Rgba32>
        {
            Color.Transparent
        };

        var workingPixels = new List<byte>();

        var cumulativeHeight = 0;
        foreach (var bank in banks)
        {
            var dims = InferDimensions(bank);
            using (var subImage = image.Clone(g =>
            {
                g.Crop(new Rectangle(0, cumulativeHeight, dims.Width, dims.Height));

            }))
            {
                FromImage(workingPixels, workingPalette, subImage, bank, blockSize, tiled, format);
            }
            cumulativeHeight += dims.Height;
        }

        var pixelArray = workingPixels.ToArray();
        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(pixelArray, workingPalette.ToArray(), width, height);
    }
}