using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class NscrDefaultPopulater : IMiscItemDefaultPopulater
{
    public MetaMiscItemId Id => MetaMiscItemId.NSCR;

    public void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem)
    {
        var item = (G2DRMiscItem)miscItem;

        var outFolder = Path.Combine(defaultDataFolder, item.LinkFolder);
        LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), outFolder);

        var (ncgr, nclr) = G2DR.LoadImgFromFolder(outFolder);
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(defaultDataFolder, item.PngFile));
    }
}
