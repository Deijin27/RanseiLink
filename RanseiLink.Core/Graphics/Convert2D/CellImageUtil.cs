using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public static class CellImageUtil
{
    private static BankDimensions InferDimensions(CellBank bank, int width = -1, int height = -1)
    {
        if (bank.Count == 0)
        {
            // if the bank is empty, we still return bank dimensoins which are
            // just the width and height with everything else as default
            // The other values will not be used, so it doesn't matter
            // The width and height should be the values passed in.
            // this will lead to a transparent, blank placeholder image to be produced.
            // but default the width and height to non-zero values
            // for the case where the user doesn't provide them to remain valid image
            return new BankDimensions(0, 0, 0, 0, width < 0 ? 1 : width, height < 0 ? 1 : height);
        }
        int minY = bank.Min(i => i.YOffset);
        int yShift = minY < 0 ? -minY : 0;
        int minX = bank.Min(i => i.XOffset);
        int xShift = minX < 0 ? -minX : 0;

        if (width < 0 || height < 0)
        {
            int maxY = bank.Max(i => i.YOffset + i.Height);
            int maxX = bank.Max(i => i.XOffset + i.Width);
            width = maxX + xShift;
            height = maxY + yShift;
        }

        return new BankDimensions(minX, minY, xShift, yShift, width, height);
    }

    private record BankDimensions(int MinX, int MinY, int XShift, int YShift, int Width, int Height);

    public static Image<Rgba32> CellToImage(Cell cell, uint blockSize, SpriteImageInfo imageInfo)
    {
        int tileOffset = cell.TileOffset << (byte)blockSize;

        var startByte = tileOffset * 0x20;
        if (imageInfo.Format == TexFormat.Pltt16)
        {
            startByte *= 2; // account for compression e.g. pokemon conquest minimaps
        }
        byte[] cellPixels = imageInfo.Pixels.Skip(startByte).Take(cell.Width * cell.Height).ToArray();

        var cellImg = ImageUtil.SpriteToImage(new SpriteImageInfo(cellPixels, imageInfo.Palette, cell.Width, cell.Height, imageInfo.IsTiled, imageInfo.Format));

        if (cell.FlipX || cell.FlipY)
        {
            cellImg.Mutate(g =>
            {
                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });
        }
        
        return cellImg;
    }

    public static IReadOnlyList<Image<Rgba32>> SingleBankToMultipleImages(CellBank bank, uint blockSize, SpriteImageInfo imageInfo)
    {
        var images = new Image<Rgba32>[bank.Count];

        imageInfo.Palette[0] = Color.Transparent;

        for (int i = 0; i < bank.Count; i++)
        {
            var cell = bank[i];
            var image = CellToImage(cell, blockSize, imageInfo);
            images[i] = image;
        }
        return images;
    }

    public static Image<Rgba32> SingleBankToImage(CellBank bank, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
    {
        var dims = InferDimensions(bank, imageInfo.Width, imageInfo.Height);

        imageInfo.Palette[0] = Color.Transparent;

        var graphic = new Image<Rgba32>(dims.Width, dims.Height);
        foreach (var cell in bank)
        {
            using (var cellImg = CellToImage(cell, blockSize, imageInfo))
            {
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
                int i = 0;
                foreach (var cell in bank)
                {
                    g.DrawText(i.ToString(), SystemFonts.CreateFont("Arial", 9), Color.Black, new PointF(cell.XOffset + 2 + dims.XShift, cell.YOffset + 2 + dims.YShift));
                    g.Draw(Pens.Solid(Color.Red, 1), new RectangleF(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift, cell.Width, cell.Height));
                    i++;
                }
            });
        }

        return graphic;
    }

    public static void SingleBankToPng(string file, CellBank bank, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
    {
        using var graphic = SingleBankToImage(bank, blockSize, imageInfo, debug);
        graphic.SaveAsPng(file);
    }


    public static Image<Rgba32> MultiBankToImage(IReadOnlyList<CellBank> banks, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }
        else if (banks.Count == 1)
        {
            return SingleBankToImage(banks[0], blockSize, imageInfo, debug);
        }
        else
        {
            var images = MultiBankToMultipleImages(banks, blockSize, imageInfo, debug);
            return ImageUtil.CombineImagesVertically(images);
        }
    }

    public static IReadOnlyList<Image<Rgba32>> MultiBankToMultipleImages(IReadOnlyList<CellBank> banks, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
    {
        return banks.Select(bank => SingleBankToImage(bank, blockSize, imageInfo, debug)).ToList();
    }

    public static IReadOnlyList<IReadOnlyList<Image<Rgba32>>> MultiBankToMultipleImageGroups(IReadOnlyCollection<CellBank> banks, uint blockSize, SpriteImageInfo imageInfo)
    {
        return banks.Select(bank => SingleBankToMultipleImages(bank, blockSize, imageInfo)).ToList();
    }

    // dupe?
    /// <summary>
    /// Calculates the pixels, colors, and cell tile offset from an image. Requires some cell parameters, such as FlipX to be pre filled.
    /// </summary>
    public static void CellFromImage(Image<Rgba32> image, Cell cell, uint blockSize, List<byte> workingPixels, List<Rgba32> workingPalette,
        bool tiled, TexFormat format)
    {
        // mutate image based on pre-configured cell parameters
        if (cell.FlipX || cell.FlipY)
        {
            image.Mutate(g =>
            {
                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });
        }

        // work out tile offset
        var startByte = workingPixels.Count;
        if (format == TexFormat.Pltt16)
        {
            startByte /= 2; // account for compression
        }
        var tileOffset = startByte / 0x20;
        cell.TileOffset = tileOffset >> (byte)blockSize;

        // get pixels from image
        var pixels = ImageUtil.SharedPalettePixelsFromImage(image, workingPalette, tiled, format, color0ToTransparent: true);
        workingPixels.AddRange(pixels);
    }
    public static void SharedSingleBankFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, CellBank bank, uint blockSize,
        List<byte> workingPixels, List<Rgba32> workingPalette,
        bool tiled, TexFormat format)
    {
        if (images.Count != bank.Count)
        {
            throw new ArgumentException($"Images did not have the same number of items as cell bank ({images.Count} vs {bank.Count})");
        }

        for (int i = 0; i < bank.Count; i++)
        {
            var image = images[i];
            var cell = bank[i];
            CellFromImage(image, cell, blockSize, workingPixels, workingPalette, tiled, format);
        }
    }

    public static SpriteImageInfo SingleBankFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, CellBank bank, uint blockSize,
        bool tiled, TexFormat format)
    {
        var workingPixels = new List<byte>();
        var workingPalette = new List<Rgba32>() { Color.Transparent };

        SharedSingleBankFromMultipleImages(images, bank, blockSize, workingPixels, workingPalette, tiled, format);

        return new SpriteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette.ToArray(),
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    public static void SharedSingleBankFromImage(Image<Rgba32> image, CellBank bank, uint blockSize,
        List<byte> workingPixels, List<Rgba32> workingPalette,
        bool tiled, TexFormat format)
    {
        var dims = InferDimensions(bank, image.Width, image.Height);

        for (int i = 0; i < bank.Count; i++)
        {
            var cell = bank[i];
            using (var cellImg = image.Clone(g =>
            {
                g.Crop(new Rectangle(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift, cell.Width, cell.Height));
            }))
            {
                CellFromImage(cellImg, cell, blockSize, workingPixels, workingPalette, tiled, format);
            }
        }
    }

    public static SpriteImageInfo SingleBankFromImage(Image<Rgba32> image, CellBank bank, uint blockSize,
        bool tiled, TexFormat format)
    {
        var workingPixels = new List<byte>();
        var workingPalette = new List<Rgba32>() { Color.Transparent };

        SharedSingleBankFromImage(image, bank, blockSize, workingPixels, workingPalette, tiled, format);

        workingPalette[0] = Color.Magenta;
        return new SpriteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette.ToArray(),
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    public static SpriteImageInfo SingleBankFromPng(string file, CellBank bank, uint blockSize, bool tiled, TexFormat format)
    {
        if (bank.Count == 0)
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return SingleBankFromImage(image, bank, blockSize, tiled, format);
    }

    public static SpriteImageInfo MultiBankFromPng(string file, IReadOnlyList<CellBank> banks, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Any(x => x.Count == 0))
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return MultiBankFromImage(image, banks, blockSize, tiled, format);
    }

    public static SpriteImageInfo MultiBankFromImage(Image<Rgba32> image, IReadOnlyList<CellBank> banks, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }
        if (banks.Count == 1)
        {
            return SingleBankFromImage(image, banks[0], blockSize, tiled, format);
        }

        var width = image.Width;
        var height = image.Height;

        var workingPalette = new List<Rgba32>
        {
            Color.Transparent
        };
        var workingPixels = new List<byte>();

        SharedPaletteMultiBankFromImage(image, banks, workingPixels, workingPalette, blockSize, tiled, format);
        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(workingPixels.ToArray(), workingPalette.ToArray(), width, height, tiled, format);
    }

    public static void SharedPaletteMultiBankFromImage(Image<Rgba32> image, IReadOnlyList<CellBank> banks, List<byte> workingPixels, List<Rgba32> workingPalette, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }

        if (banks.Count == 1)
        {
            SharedSingleBankFromImage(image, banks[0], blockSize, workingPixels, workingPalette, tiled, format);
        }

        var width = image.Width;
        var height = image.Height;

        var cumulativeHeight = 0;
        foreach (var bank in banks)
        {
            var dims = InferDimensions(bank);
            using (var subImage = image.Clone(g =>
            {
                g.Crop(new Rectangle(0, cumulativeHeight, dims.Width, dims.Height));

            }))
            {
                SharedSingleBankFromImage(subImage, bank, blockSize, workingPixels, workingPalette, tiled, format);
            }
            cumulativeHeight += dims.Height;
        }
    }
    public static SpriteImageInfo MultiBankFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, IReadOnlyList<CellBank> banks, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Count == 0)
        {
            throw new ArgumentException("Can't load image with no cell banks");
        }
        if (banks.Count != images.Count)
        {
            throw new ArgumentException($"Images did not have the same number of items as banks ({images.Count} vs {banks.Count})");
        }

        var workingPalette = new List<Rgba32> { Color.Transparent };
        var workingPixels = new List<byte>();
        for (int i = 0; i < banks.Count; i++)
        {
            var bank = banks[i];
            var image = images[i];
            SharedSingleBankFromImage(image, bank, blockSize, workingPixels, workingPalette, tiled, format);
        }

        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette.ToArray(),
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }


    public static CellSize GetCellSize(Shape shape, Scale scale)
    {
        if (!s_scaleShapeLookup.TryGetValue(shape, out var scaleDict))
        {
            throw new ArgumentException($"Invalid cell shape {shape}");
        }
        if (!scaleDict.TryGetValue(scale, out var size))
        {
            throw new ArgumentException($"Invalid cell scale {scale}");
        }
        return size;
    }

    public static CellSize? GetCellSize(int width, int height)
    {
        if (s_widthHeightLookup.TryGetValue(width, out var heightDict) && heightDict.TryGetValue(height, out var size))
        {
            return size;
        }
        return null;
    }

    private static readonly Dictionary<Shape, Dictionary<Scale, CellSize>> s_scaleShapeLookup;
    private static readonly Dictionary<int, Dictionary<int, CellSize>> s_widthHeightLookup;

    static CellImageUtil()
    {
        ValidCellSizes = new[]
        {
            new CellSize(Shape.Square, Scale.Small, 8, 8),
            new CellSize(Shape.Square, Scale.Medium, 16, 16),
            new CellSize(Shape.Square, Scale.Large, 32, 32),
            new CellSize(Shape.Square, Scale.XLarge, 64, 64),

            new CellSize(Shape.Wide, Scale.Small, 16, 8),
            new CellSize(Shape.Wide, Scale.Medium, 32, 8),
            new CellSize(Shape.Wide, Scale.Large, 32, 16),
            new CellSize(Shape.Wide, Scale.XLarge, 64, 32),

            new CellSize(Shape.Tall, Scale.Small, 8, 16),
            new CellSize(Shape.Tall, Scale.Medium, 8, 32),
            new CellSize(Shape.Tall, Scale.Large, 16, 32),
            new CellSize(Shape.Tall, Scale.XLarge, 32, 64),
        };

        s_scaleShapeLookup = new();
        s_widthHeightLookup = new();

        foreach (var size in ValidCellSizes)
        {
            if (!s_scaleShapeLookup.TryGetValue(size.Shape, out var scaleDict))
            {
                scaleDict = new();
                s_scaleShapeLookup[size.Shape] = scaleDict;
            }
            scaleDict[size.Scale] = size;

            if (!s_widthHeightLookup.TryGetValue(size.Width, out var heightDict))
            {
                heightDict = new();
                s_widthHeightLookup[size.Width] = heightDict;
            }
            heightDict[size.Height] = size;
        }

    }

    public static CellSize[] ValidCellSizes { get; }
}

public record CellSize(Shape Shape, Scale Scale, int Width, int Height);