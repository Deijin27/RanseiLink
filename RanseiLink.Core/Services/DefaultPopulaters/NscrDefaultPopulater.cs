using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class NscrDefaultPopulater : IMiscItemDefaultPopulater
{
    public MetaMiscItemId Id => MetaMiscItemId.NSCR;

    public void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem)
    {
        var item = (G2DRMiscItem)miscItem;

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
