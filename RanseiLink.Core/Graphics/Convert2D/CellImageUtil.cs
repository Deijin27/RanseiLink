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
    private static BankDimensions InferDimensions(Cell[] bank, int width = -1, int height = -1)
    {
        if (bank.Length == 0)
        {
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
            width = maxX + xShift; // maybe this is correct?
            height = maxY + yShift;
        }

        return new BankDimensions(minX, minY, xShift, yShift, width, height);
    }

    private record BankDimensions(int MinX, int MinY, int XShift, int YShift, int Width, int Height);

    public static Image<Rgba32> SingleBankToImage(Cell[] bank, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
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
            if (imageInfo.Format == TexFormat.Pltt16)
            {
                startByte *= 2; // account for compression e.g. pokemon conquest minimaps
            }
            byte[] cellPixels = imageInfo.Pixels.Skip(startByte).Take(cell.Width * cell.Height).ToArray();

            using (var cellImg = ImageUtil.SpriteToImage(new SpriteImageInfo(cellPixels, palette32, cell.Width, cell.Height, imageInfo.IsTiled, imageInfo.Format)))
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

    public static void SingleBankToPng(string file, Cell[] bank, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
    {
        using var graphic = SingleBankToImage(bank, blockSize, imageInfo, debug);
        graphic.SaveAsPng(file);
    }

    public static Image<Rgba32> MultiBankToImage(IList<Cell[]> banks, uint blockSize, SpriteImageInfo imageInfo, bool debug = false)
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
            var images = banks.Select(bank => SingleBankToImage(bank, blockSize, imageInfo, debug)).ToList();

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

    private static void SharedPaletteBankFromImage(List<byte> workingPixels, List<Rgba32> workingPalette, Image<Rgba32> image, Cell[] bank, uint blockSize, bool tiled, TexFormat format)
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
                byte[] cellPixels = ImageUtil.SharedPalettePixelsFromImage(cellImg, workingPalette, tiled, format, color0ToTransparent: true);
                workingPixels.AddRange(cellPixels);
            }
        }
    }

    public static SpriteImageInfo SingleBankFromPng(string file, Cell[] bank, uint blockSize, bool tiled, TexFormat format)
    {
        if (bank.Length == 0)
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return SingleBankFromImage(image, bank, blockSize, tiled, format);
    }

    public static SpriteImageInfo SingleBankFromImage(Image<Rgba32> image, Cell[] bank, uint blockSize, bool tiled, TexFormat format)
    {
        var width = image.Width;
        var height = image.Height;

        var workingPalette = new List<Rgba32>
        {
            Color.Transparent
        };

        var workingPixels = new List<byte>();

        SharedPaletteBankFromImage(workingPixels, workingPalette, image, bank, blockSize, tiled, format);

        var pixelArray = workingPixels.ToArray();
        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(pixelArray, workingPalette.ToArray(), width, height, tiled, format);
    }

    public static SpriteImageInfo MultiBankFromPng(string file, IList<Cell[]> banks, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Any(x => x.Length == 0))
        {
            throw new Exception($"Tried to load png with empty bank (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return MultiBankFromImage(image, banks, blockSize, tiled, format);
    }

    public static SpriteImageInfo MultiBankFromImage(Image<Rgba32> image, IList<Cell[]> banks, uint blockSize, bool tiled, TexFormat format)
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

        var pixelArray = SharedPaletteMultiBankFromImage(image, banks, workingPalette, blockSize, tiled, format);
        workingPalette[0] = Color.Magenta;

        return new SpriteImageInfo(pixelArray, workingPalette.ToArray(), width, height, tiled, format);
    }

    public static byte[] SharedPaletteMultiBankFromImage(Image<Rgba32> image, IList<Cell[]> banks, List<Rgba32> workingPalette, uint blockSize, bool tiled, TexFormat format)
    {
        if (banks.Count == 0)
        {
            throw new Exception("Can't load image with no cell banks");
        }
        var workingPixels = new List<byte>();

        if (banks.Count == 1)
        {
            SharedPaletteBankFromImage(workingPixels, workingPalette, image, banks[0], blockSize, tiled, format);
            return workingPixels.ToArray();
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
                SharedPaletteBankFromImage(workingPixels, workingPalette, subImage, bank, blockSize, tiled, format);
            }
            cumulativeHeight += dims.Height;
        }

        var pixelArray = workingPixels.ToArray();

        return pixelArray;
    }

}