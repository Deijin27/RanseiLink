using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

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
        
        ImageUtil.SaveAsPng(
            file: pngFile,
            bank: ncer.CellBanks.Banks[0],
            blockSize: ncer.CellBanks.BlockSize,
            imageInfo: new SpriteImageInfo(
                pixels: ncgr.Pixels.Data, 
                palette: RawPalette.To32bitColors(nclr.Palettes.Palette), 
                width: width,
                height: height
                ),
            debug: false,
            tiled: ncgr.Pixels.IsTiled
            );
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

        ImageUtil.SaveAsPng(
            file: pngFile,
            imageInfo: new SpriteImageInfo(
                pixels: ncgr.Pixels.Data,
                palette: RawPalette.To32bitColors(nclr.Palettes.Palette),
                width: ncgr.Pixels.TilesPerRow * 8,
                height: ncgr.Pixels.TilesPerColumn * 8
                ),
            tiled: true
            );
    }
}
