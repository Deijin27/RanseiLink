using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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
                case MetaMiscItemId.IconInstS:
                    ProcessIconInstS(defaultDataFolder, (BuildingIconSmallMiscItem)item);
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

    private void ProcessIconInstS(string defaultDataFolder, BuildingIconSmallMiscItem item)
    {
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

            LINK.Unpack(link, linkFolder, true, 4);
            linkFolders.Add(id, linkFolder);
        }

        // Load the palette
        // one palette stored with the first image is shared between all images
        if (!linkFolders.TryGetValue(0, out var palFolder))
        {
            throw new Exception("Palette folder not found for IconInstS");
        }
        var pal = Path.Combine(palFolder, "0004.nclr");
        var palFileInfo = new FileInfo(pal);
        string? nclrPath = null;
        if (palFileInfo.Exists && palFileInfo.Length > 0)
        {
            nclrPath = pal;
        }
        else
        {
            throw new Exception("Palette not found in its folder for IconInstS");
        }
        var nclr = NCLR.Load(nclrPath);
        var palette = PaletteUtil.To32bitColors(nclr.Palettes.Palette);

        // Load the ncer and ncgrs, then load all the images
        const int width = 32;
        const int height = 32;
        var maxId = linkFolders.Select(x => x.Key).Max();
        using var combinedImage = new Image<Rgba32>(width, (maxId + 1) * height);
        combinedImage.Mutate(g =>
        {
            foreach (var (linkId, linkFolder) in linkFolders)
            {
                var ncer = NCER.Load(Path.Combine(linkFolder, "0002.ncer"));
                var ncgr = NCGR.Load(Path.Combine(linkFolder, "0003.ncgr"));

                // Not all building ids have icons
                // to facilitate potentially filling in these gaps in the future,
                // we enforce the 32x32 size, and add gaps
                using var image = CellImageUtil.MultiBankToImage(
                   banks: ncer.CellBanks.Banks,
                   blockSize: ncer.CellBanks.BlockSize,
                   imageInfo: new SpriteImageInfo(
                       Pixels: ncgr.Pixels.Data,
                       Palette: palette,
                       Width: width,
                       Height: height,
                       IsTiled: ncgr.Pixels.IsTiled,
                       Format: ncgr.Pixels.Format
                       ),
                   debug: false
                   );
                g.DrawImage(image, new Point(0, linkId * height), 1);
            }
        });

        combinedImage.SaveAsPng(pngFile);
    }

    
}

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
}
