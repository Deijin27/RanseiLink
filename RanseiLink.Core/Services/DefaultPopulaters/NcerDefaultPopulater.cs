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

        LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), Path.Combine(defaultDataFolder, item.LinkFolder), true, 4);
        var ncgrPath = Path.Combine(defaultDataFolder, item.Ncgr);
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(defaultDataFolder, item.NcgrAlt);
        }

        using var image = NitroImageUtil.NcerToImage(
            ncer: NCER.Load(Path.Combine(defaultDataFolder, item.Ncer)),
            ncgr: NCGR.Load(ncgrPath),
            nclr: NCLR.Load(Path.Combine(defaultDataFolder, item.Nclr))
            );

        image.SaveAsPng(Path.Combine(defaultDataFolder, item.PngFile));
    }

    
}
