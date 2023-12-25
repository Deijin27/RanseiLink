using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class NcerDefaultPopulater : IMiscItemDefaultPopulater
{
    public MetaMiscItemId Id => MetaMiscItemId.NCER;

    public void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem)
    {
        var item = (G2DRMiscItem)miscItem;

        string outFolder = Path.Combine(defaultDataFolder, item.LinkFolder);
        LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), outFolder);

        var (ncer, ncgr, nclr) = G2DR.LoadCellImgFromFolder(outFolder, NcgrSlot.Infer);

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr);

        image.SaveAsPng(Path.Combine(defaultDataFolder, item.PngFile));
    }

    
}
