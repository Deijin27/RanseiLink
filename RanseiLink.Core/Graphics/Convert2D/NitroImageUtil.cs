using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace RanseiLink.Core.Graphics;

public static class NitroImageUtil
{
    public static (int width, int height) InferDimsTiled(NCGR ncgr, int tilesize)
    {
        int width, height;
        if (ncgr.Pixels.TilesPerColumn == -1 || ncgr.Pixels.TilesPerRow == -1)
        {
            width = -1;
            height = -1;
        }
        else
        {
            width = ncgr.Pixels.TilesPerRow * tilesize;
            height = ncgr.Pixels.TilesPerColumn * tilesize;
        }
        return (width, height);
    }

    public static Image<Rgba32> NcerToImage(NCER ncer, NCGR ncgr, NCLR nclr, int width = -1, int height = -1, PositionRelativeTo prt = PositionRelativeTo.TopLeft, bool debug = false)
    {
        if (width < 0 || height < 0)
        {
            (width, height) = InferDimsTiled(ncgr, 8);
        }

        return CellImageUtil.MultiBankToImage(
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            imageInfo: new MultiPaletteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: new PaletteCollection(nclr.Palettes.Palette, nclr.Palettes.Format, true),
                Width: width,
                Height: height,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
                ),
            prt: prt,
            debug: debug
            );
    }

    public static IReadOnlyList<Image<Rgba32>> NcerToMultipleImages(NCER ncer, NCGR ncgr, NCLR nclr, int width = -1, int height = -1, PositionRelativeTo prt = PositionRelativeTo.TopLeft)
    {
        return CellImageUtil.MultiBankToMultipleImages(
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            imageInfo: new MultiPaletteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: new PaletteCollection(nclr.Palettes.Palette, nclr.Palettes.Format, true),
                Width: width,
                Height: height,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
                ),
            prt: prt,
            debug: false
            );
    }

    public static IReadOnlyList<IReadOnlyList<Image<Rgba32>>> NcerToMultipleImageGroups(NCER ncer, NCGR ncgr, NCLR nclr)
    {
        return CellImageUtil.MultiBankToMultipleImageGroups(
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            imageInfo: new MultiPaletteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: new PaletteCollection(nclr.Palettes.Palette, nclr.Palettes.Format, true),
                Width: -1,
                Height: -1,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
                )
            );
    }

    public static Image<Rgba32> NcgrToImage(NCGR ncgr, NCLR nclr)
    {
        return ImageUtil.SpriteToImage(
            new SpriteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: new Palette(nclr.Palettes.Palette, false),
                Width: ncgr.Pixels.TilesPerRow * 8,
                Height: ncgr.Pixels.TilesPerColumn * 8,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
            ));
    }

    /// <summary>
    /// Import image data into a pre-existing ncer. This will modify the ncgr and nclr.
    /// </summary>
    public static void NcerFromImage(NCER ncer, NCGR ncgr, NCLR nclr, Image<Rgba32> image, int width = -1, int height = -1, PositionRelativeTo prt = PositionRelativeTo.TopLeft)
    {
        if (width < 0 || height < 0)
        {
            (width, height) = InferDimsTiled(ncgr, 8);
        }
        var imageInfo = CellImageUtil.MultiBankFromImage(
                image: image,
                banks: ncer.CellBanks.Banks,
                blockSize: ncer.CellBanks.BlockSize,
                tiled: ncgr.Pixels.IsTiled,
                format: ncgr.Pixels.Format,
                width: width,
                height: height,
                prt: prt
                );
        ProcessNcerImageInfo(imageInfo, ncgr, nclr);
    }

    private static void ProcessNcerImageInfo(MultiPaletteImageInfo imageInfo, NCGR ncgr, NCLR nclr)
    {
        ncgr.Pixels.Data = imageInfo.Pixels;
        if (ncgr.Pixels.TilesPerColumn != -1)
        {
            ncgr.Pixels.TilesPerRow = (short)(imageInfo.Width / 8);
            ncgr.Pixels.TilesPerColumn = (short)(imageInfo.Height / 8);
        }

        var palSize = imageInfo.Format.PaletteSize();
        if (imageInfo.Palette.Any(x => x.Count > palSize))
        {
            throw new InvalidPaletteException($"Palette length exceeds allowed length {palSize} when importing image");
        }

        Rgb15[] mergedPalette;
        if (imageInfo.Palette.Count == 1)
        {
            mergedPalette = PaletteUtil.From32bitColors(imageInfo.Palette[0]);
        }
        else
        {
            mergedPalette = new Rgb15[imageInfo.Palette.Count * palSize];
            for (int i = 0; i < imageInfo.Palette.Count; i++)
            {
                var pal = PaletteUtil.From32bitColors(imageInfo.Palette[i]);
                pal.CopyTo(mergedPalette, i * palSize);
            }
        }
        nclr.Palettes.Palette = mergedPalette;
        if (nclr.PaletteCollectionMap != null)
        {
            nclr.PaletteCollectionMap.Palettes.Clear();
            for (int i = 0; i < imageInfo.Palette.Count; i++)
            {
                nclr.PaletteCollectionMap.Palettes.Add((ushort)i);
            }
        }
    }

    public static void NcerFromMultipleImages(NCER ncer, NCGR ncgr, NCLR nclr, IReadOnlyList<Image<Rgba32>> images, PositionRelativeTo prt = PositionRelativeTo.TopLeft)
    {
        var imageInfo = CellImageUtil.MultiBankFromMultipleImages(
            images: images,
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            tiled: ncgr.Pixels.IsTiled,
            format: ncgr.Pixels.Format,
            prt: prt
            );

        ProcessNcerImageInfo(imageInfo, ncgr, nclr);
    }

    public static void NcerFromMultipleImageGroups(NCER ncer, NCGR ncgr, NCLR nclr, IReadOnlyList<IReadOnlyList<Image<Rgba32>>> imageGroups)
    {
        var imageInfo = CellImageUtil.MultiBankFromMultipleImageGroups(
            imageGroups: imageGroups,
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            tiled: ncgr.Pixels.IsTiled,
            format: ncgr.Pixels.Format
            );

        ProcessNcerImageInfo(imageInfo, ncgr, nclr);
    }

    /// <summary>
    /// Import image data into a pre-existing ncgr. This will modify the ncgr and nclr.
    /// </summary>
    public static void NcgrFromImage(NCGR ncgr, NCLR nclr, Image<Rgba32> image, bool color0ToTransparent = true)
    {
        var imageInfo = ImageUtil.SpriteFromImage(image, ncgr.Pixels.IsTiled, ncgr.Pixels.Format, color0ToTransparent: color0ToTransparent);
        ncgr.Pixels.Data = imageInfo.Pixels;
        ncgr.Pixels.TilesPerRow = (short)(imageInfo.Width / 8);
        ncgr.Pixels.TilesPerColumn = (short)(imageInfo.Height / 8);

        var newPalette = PaletteUtil.From32bitColors(imageInfo.Palette);
        if (newPalette.Length > nclr.Palettes.Palette.Length)
        {
            // this should not be hit because it should be filtered out by the palette simplifier
            throw new InvalidPaletteException($"Palette length exceeds current palette when importing image {newPalette.Length} vs {nclr.Palettes.Palette.Length}");
        }
        newPalette.CopyTo(nclr.Palettes.Palette, 0);
    }
}
