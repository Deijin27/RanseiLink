﻿using SixLabors.ImageSharp.PixelFormats;
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

    public static Image<Rgba32> NcerToImage(NCER ncer, NCGR ncgr, NCLR nclr, int width = -1, int height = -1)
    {
        if (width < 0 || height < 0)
        {
            (width, height) = InferDimsTiled(ncgr, 8);
        }

        return CellImageUtil.MultiBankToImage(
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            imageInfo: new SpriteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: PaletteUtil.To32bitColors(nclr.Palettes.Palette),
                Width: width,
                Height: height,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
                ),
            debug: false
            );
    }

    public static Image<Rgba32> NcgrToImage(NCGR ncgr, NCLR nclr)
    {
        return ImageUtil.SpriteToImage(
            new SpriteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: PaletteUtil.To32bitColors(nclr.Palettes.Palette),
                Width: ncgr.Pixels.TilesPerRow * 8,
                Height: ncgr.Pixels.TilesPerColumn * 8,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
            ));
    }

    /// <summary>
    /// Import image data into a pre-existing ncer. This will modify the ncgr and nclr.
    /// </summary>
    public static void NcerImport(NCER ncer, NCGR ncgr, NCLR nclr, string png)
    {
        var imageInfo = CellImageUtil.MultiBankFromPng(
            file: png,
            banks: ncer.CellBanks.Banks,
            blockSize: ncer.CellBanks.BlockSize,
            tiled: ncgr.Pixels.IsTiled,
            format: ncgr.Pixels.Format
            );
        ncgr.Pixels.Data = imageInfo.Pixels;
        if (ncgr.Pixels.TilesPerColumn != -1)
        {
            ncgr.Pixels.TilesPerRow = (short)(imageInfo.Width / 8);
            ncgr.Pixels.TilesPerColumn = (short)(imageInfo.Height / 8);
        }

        var newPalette = PaletteUtil.From32bitColors(imageInfo.Palette);
        if (newPalette.Length > nclr.Palettes.Palette.Length)
        {
            throw new InvalidPaletteException($"Palette length exceeds current palette when importing '{png}'");
        }
        newPalette.CopyTo(nclr.Palettes.Palette, 0);
    }

    /// <summary>
    /// Import image data into a pre-existing ncgr. This will modify the ncgr and nclr.
    /// </summary>
    public static void NcgrImport(NCGR ncgr, NCLR nclr, string png, bool color0ToTransparent = true)
    {
        var imageInfo = ImageUtil.SpriteFromPng(png, ncgr.Pixels.IsTiled, ncgr.Pixels.Format, color0ToTransparent: color0ToTransparent);
        ncgr.Pixels.Data = imageInfo.Pixels;
        ncgr.Pixels.TilesPerRow = (short)(imageInfo.Width / 8);
        ncgr.Pixels.TilesPerColumn = (short)(imageInfo.Height / 8);

        var newPalette = PaletteUtil.From32bitColors(imageInfo.Palette);
        if (newPalette.Length > nclr.Palettes.Palette.Length)
        {
            // this should not be hit because it should be filtered out by the palette simplifier
            throw new InvalidPaletteException($"Palette length exceeds current palette when importing '{png}'");
        }
        newPalette.CopyTo(nclr.Palettes.Palette, 0);
    }
}