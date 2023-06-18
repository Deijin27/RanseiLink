using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public class MiscDefaultPopulater : IGraphicTypeDefaultPopulater
{
    public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
    {
        if (gInfo.MetaType != MetaSpriteType.Misc)
        {
            return;
        }

        var miscInfo = (MiscConstants)gInfo;

        foreach (var item in miscInfo.Items)
        {
            switch (item.MetaId)
            {
                case MetaMiscItemId.NCER:
                    ProcessNcer(defaultDataFolder, (G2DRMiscItem)item);
                    break;
                case MetaMiscItemId.NSCR:
                    ProcessNscr(defaultDataFolder, (G2DRMiscItem)item);
                    break;
            }
        }
    }

    private void ProcessNcer(string defaultDataFolder, G2DRMiscItem item)
    {
        string pngFile = Path.Combine(defaultDataFolder, item.PngFile);

        LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), Path.Combine(defaultDataFolder, item.LinkFolder), true, 4);
        var ncer = NCER.Load(Path.Combine(defaultDataFolder, item.Ncer));
        var ncgrPath = Path.Combine(defaultDataFolder, item.Ncgr);
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(defaultDataFolder, item.NcgrAlt);
        }
        var ncgr = NCGR.Load(ncgrPath);
        var nclr = NCLR.Load(Path.Combine(defaultDataFolder, item.Nclr));

        int width;
        int height;

        if (ncgr.Pixels.TilesPerColumn == -1 || ncgr.Pixels.TilesPerRow == -1)
        {
            width = -1;
            height = -1;
        }
        else
        {
            width = ncgr.Pixels.TilesPerRow * 8;
            height = ncgr.Pixels.TilesPerColumn * 8;
        }

        using var image = CellImageUtil.MultiBankToImage(
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

        image.SaveAsPng(pngFile);
    }

    private void ProcessNscr(string defaultDataFolder, G2DRMiscItem item)
    {
        string pngFile = Path.Combine(defaultDataFolder, item.PngFile);

        LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), Path.Combine(defaultDataFolder, item.LinkFolder), true, 4);

        var ncgrPath = Path.Combine(defaultDataFolder, item.Ncgr);
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(defaultDataFolder, item.NcgrAlt);
        }
        var ncgr = NCGR.Load(ncgrPath);
        var nclr = NCLR.Load(Path.Combine(defaultDataFolder, item.Nclr));

        ImageUtil.SpriteToPng(
            file: pngFile,
            imageInfo: new SpriteImageInfo(
                Pixels: ncgr.Pixels.Data,
                Palette: PaletteUtil.To32bitColors(nclr.Palettes.Palette),
                Width: ncgr.Pixels.TilesPerRow * 8,
                Height: ncgr.Pixels.TilesPerColumn * 8,
                IsTiled: ncgr.Pixels.IsTiled,
                Format: ncgr.Pixels.Format
            ));
    }
}
