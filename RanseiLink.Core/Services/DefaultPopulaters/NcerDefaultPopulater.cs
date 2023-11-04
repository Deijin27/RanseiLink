using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class NcerDefaultPopulater : IMiscItemDefaultPopulater
{
    public MetaMiscItemId Id => MetaMiscItemId.NCER;

    public void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem)
    {
        var item = (G2DRMiscItem)miscItem;

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

        var (width, height) = NitroImageUtil.InferDimsTiled(ncgr, 8);

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
}
