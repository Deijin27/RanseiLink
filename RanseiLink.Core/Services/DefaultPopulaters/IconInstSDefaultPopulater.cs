using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class IconInstSDefaultPopulater : IMiscItemDefaultPopulater
{
    public MetaMiscItemId Id => MetaMiscItemId.IconInstS;

    public void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo, MiscItem miscItem)
    {
        var item = (BuildingIconSmallMiscItem)miscItem;

        string pngFile = Path.Combine(defaultDataFolder, item.PngFile);

        var containingFolder = Path.Combine(defaultDataFolder, item.ContainingFolder);

        // unpack links
        var rx = new Regex(@"03_05_parts_shisetsuicon_s_(\d\d)\.G2DR");
        var linkFiles = Directory.GetFiles(containingFolder, "*.G2DR");
        var linkFolders = new Dictionary<int, string>();
        foreach (var link in linkFiles)
        {
            var linkFileName = Path.GetFileName(link);
            var linkFolder = Path.Combine(Path.GetDirectoryName(link)!, Path.GetFileNameWithoutExtension(link) + "-Unpacked");

            var match = rx.Match(linkFileName);
            if (!match.Success)
            {
                throw new Exception($"Found unexpected link file when processing IconInstS '{linkFileName}'");
            }
            var id = int.Parse(match.Groups[1].Value);

            LINK.Unpack(link, linkFolder);
            linkFolders.Add(id, linkFolder);
        }

        // Load the palette
        // one palette stored with the first image is shared between all images
        if (!linkFolders.TryGetValue(0, out var palFolder))
        {
            throw new Exception("Palette folder not found for IconInstS");
        }
        var nclr = G2DR.LoadPaletteFromFolder(palFolder);
        var palette = new PaletteCollection(nclr.Palettes.Palette, nclr.Palettes.Format, true);

        // Load the ncer and ncgrs, then load all the images
        const int width = 32;
        const int height = 32;
        var maxId = linkFolders.Select(x => x.Key).Max();
        using var combinedImage = new Image<Rgba32>(width, (maxId + 1) * height);
        combinedImage.Mutate(g =>
        {
            foreach (var (linkId, linkFolder) in linkFolders)
            {
                var ncgr = G2DR.LoadPixelsFromFolder(linkFolder, NcgrSlot.Slot3);
                var ncer = G2DR.LoadCellFromFolder(linkFolder);

                // Not all building ids have icons
                // to facilitate potentially filling in these gaps in the future,
                // we enforce the 32x32 size, and add gaps
                using var image = CellImageUtil.MultiBankToImage(
                   banks: ncer.Clusters.Clusters,
                   blockSize: ncer.Clusters.BlockSize,
                   imageInfo: new MultiPaletteImageInfo(
                       Pixels: ncgr.Pixels.Data,
                       Palette: palette, // <-- it's probably worth using this over NitroImageUtil because we can load the palette only once
                       Width: width,
                       Height: height,
                       IsTiled: ncgr.Pixels.IsTiled,
                       Format: ncgr.Pixels.Format
                       ),
                   ModPatchBuilders.IconInstSPatchBuilder.Settings
                   );
                g.DrawImage(image, new Point(0, linkId * height), 1);
            }
        });

        combinedImage.SaveAsPng(pngFile);
    }
}
