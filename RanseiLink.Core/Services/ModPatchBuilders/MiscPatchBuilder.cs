using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.DefaultPopulaters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static RanseiLink.Core.Graphics.NSTEX;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public class MiscPatchBuilder : IGraphicTypePatchBuilder
{
    private readonly IOverrideDataProvider _overrideSpriteProvider;
    private readonly string _graphicsProviderFolder;
    public MiscPatchBuilder(IOverrideDataProvider overrideSpriteProvider, ModInfo mod)
    {
        _overrideSpriteProvider = overrideSpriteProvider;
        _graphicsProviderFolder = Constants.DefaultDataFolder(mod.GameCode);
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
    {
        if (gInfo.MetaType != MetaSpriteType.Misc)
        {
            return;
        }

        var miscInfo = (MiscConstants)gInfo;

        foreach (var item in miscInfo.Items)
        {
            var spriteFile = _overrideSpriteProvider.GetSpriteFile(miscInfo.Type, item.Id);
            if (!spriteFile.IsOverride)
            {
                continue;
            }

            switch (item.MetaId)
            {
                case MetaMiscItemId.NCER:
                    ProcessNcer(filesToPatch, (G2DRMiscItem)item, spriteFile.File);
                    break;
                case MetaMiscItemId.NSCR:
                    ProcessNscr(filesToPatch, (G2DRMiscItem)item, spriteFile.File);
                    break;
                case MetaMiscItemId.IconInstS:
                    ProcessIconInstS(filesToPatch, (BuildingIconSmallMiscItem)item, spriteFile.File);
                    break;
            }
        }
    }

    private void ProcessNcer(ConcurrentBag<FileToPatch> filesToPatch, G2DRMiscItem item, string pngFile)
    {
        var tempDir = FileUtil.GetTemporaryDirectory();

        FileUtil.CopyFilesRecursively(Path.Combine(_graphicsProviderFolder, item.LinkFolder), tempDir);
        File.Delete(Path.Combine(tempDir, Path.GetFileName(item.PngFile)));

        // load up provider data
        var ncer = NCER.Load(Path.Combine(tempDir, Path.GetFileName(item.Ncer)));

        var ncgrPath = Path.Combine(tempDir, Path.GetFileName(item.Ncgr));
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(tempDir, Path.GetFileName(item.NcgrAlt));
        }
        var ncgr = NCGR.Load(ncgrPath);

        var nclrPath = Path.Combine(tempDir, Path.GetFileName(item.Nclr));
        var nclr = NCLR.Load(nclrPath);

        // load up the png and replace provider data with new image data
        var imageInfo = CellImageUtil.MultiBankFromPng(
            file: pngFile, 
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
        if (newPalette.Length > item.PaletteCapacity)
        {
            // this should not be hit because it should be filtered out by the palette simplifier
            throw new Exception($"Invalid palette length for misc sprite {item.Id}");
        }
        newPalette.CopyTo(nclr.Palettes.Palette, 0);

        // save the modified provider files
        ncgr.Save(ncgrPath);
        nclr.Save(nclrPath);

        // make the link 
        var tempLink = Path.GetTempFileName();
        LINK.Pack(tempDir, tempLink);
        Directory.Delete(tempDir, true);

        filesToPatch.Add(new FileToPatch(item.Link, tempLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }

    private void ProcessNscr(ConcurrentBag<FileToPatch> filesToPatch, G2DRMiscItem item, string pngFile)
    {
        var tempDir = FileUtil.GetTemporaryDirectory();

        FileUtil.CopyFilesRecursively(Path.Combine(_graphicsProviderFolder, item.LinkFolder), tempDir);
        File.Delete(Path.Combine(tempDir, Path.GetFileName(item.PngFile)));

        // load up provider data
        var ncgrPath = Path.Combine(tempDir, Path.GetFileName(item.Ncgr));
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(tempDir, Path.GetFileName(item.NcgrAlt));
        }
        var ncgr = NCGR.Load(ncgrPath);

        var nclrPath = Path.Combine(tempDir, Path.GetFileName(item.Nclr));
        var nclr = NCLR.Load(nclrPath);

        // load up the png and replace provider data with new image data
        var imageInfo = ImageUtil.SpriteFromPng(pngFile, ncgr.Pixels.IsTiled, ncgr.Pixels.Format, color0ToTransparent: true);
        ncgr.Pixels.Data = imageInfo.Pixels;
        ncgr.Pixels.TilesPerRow = (short)(imageInfo.Width / 8);
        ncgr.Pixels.TilesPerColumn = (short)(imageInfo.Height / 8);

        var newPalette = PaletteUtil.From32bitColors(imageInfo.Palette);
        if (newPalette.Length > item.PaletteCapacity)
        {
            // this should not be hit because it should be filtered out by the palette simplifier
            throw new Exception($"Invalid palette length for misc sprite {item.Id}");
        }
        newPalette.CopyTo(nclr.Palettes.Palette, 0);

        // save the modified provider files
        ncgr.Save(ncgrPath);
        nclr.Save(nclrPath);

        // make the link 
        var tempLink = Path.GetTempFileName();
        LINK.Pack(tempDir, tempLink);
        Directory.Delete(tempDir, true);

        filesToPatch.Add(new FileToPatch(item.Link, tempLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }

    /// <summary>
    /// TODO
    /// to maintain backwards compatibility, which keeping future options open to fill the 
    /// gaps which don't have a file, we gotta enfore the 32x32, then leave gaps of that for the missing images
    /// </summary>
    private void ProcessIconInstS(ConcurrentBag<FileToPatch> filesToPatch, BuildingIconSmallMiscItem item, string pngFile)
    {
        var parentTempDir = FileUtil.GetTemporaryDirectory();

        var containingFolder = Path.Combine(_graphicsProviderFolder, item.ContainingFolder);
        using var image = Image.Load<Rgba32>(pngFile);
        var images = new List<Image<Rgba32>>();

        // Get the link files. If we supported adding additional files
        // this would need to work differently. I assume all the ncers
        // are similar to identical, so it should be do-able.
        var rx = new Regex(@"03_05_parts_shisetsuicon_s_(\d\d)\.G2DR");
        var linkFiles = Directory.GetFiles(containingFolder, "*.G2DR");
        var workingPalette = new List<Rgba32>() { Color.Transparent };
        string? nclrPath = null;

        // Modify the ncgrs and build the shared palette
        foreach (var link in linkFiles)
        {
            var linkFileName = Path.GetFileName(link);
            var match = rx.Match(linkFileName);
            if (!match.Success)
            {
                throw new Exception($"Found unexpected link file when processing IconInstS '{linkFileName}'");
            }
            var id = int.Parse(match.Groups[1].Value);

            var tempDir = Path.Combine(parentTempDir, linkFileName);
            Directory.CreateDirectory(tempDir);
            FileUtil.CopyFilesRecursively(
                sourcePath: Path.Combine(Path.GetDirectoryName(link)!, Path.GetFileNameWithoutExtension(link) + "-Unpacked"), 
                targetPath: tempDir
                );

            var ncgrPath = Path.Combine(tempDir, "0003.ncgr");
            var ncgr = NCGR.Load(ncgrPath);
            const int width = 32;
            const int height = 32;

            var ncer = NCER.Load(Path.Combine(tempDir, "0002.ncer"));

            // Crop the part of the image for this link
            using var subImage = image.Clone(g => 
                g.Crop(new Rectangle(0, (id * height), width, height)));

            var pixels = CellImageUtil.SharedPaletteMultiBankFromImage(
                image: subImage,
                ncer.CellBanks.Banks,
                workingPalette,
                ncer.CellBanks.BlockSize,
                tiled: ncgr.Pixels.IsTiled,
                format: ncgr.Pixels.Format
                );

            if (ncgr.Pixels.Data.Length != pixels.Length)
            {
                throw new Exception("Image was different size when processing IconInstS");
            }
            ncgr.Pixels.Data = pixels;
            ncgr.Save(ncgrPath);

            if (id == 0)
            {
                // this is the one where the palette is stored
                // so we store a reference to it. The later, when we have calculated the
                // full palette, we can write it to the file
                var pal = Path.Combine(tempDir, "0004.nclr");
                var palFileInfo = new FileInfo(pal);
                if (palFileInfo.Exists && palFileInfo.Length > 0)
                {
                    nclrPath = pal;
                }
            }

        }

        // Save the palette
        // if the palette is too big this is an error
        // if the palette is too small it will be scaled to the right 
        // length via the Array.Copy
        if (nclrPath == null)
        {
            throw new FileNotFoundException("Palette not found for IconInstS");
        }
        var nclr = NCLR.Load(nclrPath);
        var newPalette = PaletteUtil.From32bitColors(workingPalette);
        if (newPalette.Length > nclr.Palettes.Palette.Length)
        {
            throw new InvalidPaletteException($"Palette is bigger than allowed for in IconInstS ({newPalette.Length} vs {nclr.Palettes.Palette.Length})");
        }
        Array.Copy(newPalette, nclr.Palettes.Palette, newPalette.Length);

        // Generate the LINKs for patching
        // Temp dirs are stored within the parent dir and are deleted all at once an the end
        foreach (var link in linkFiles)
        {
            var linkFileName = Path.GetFileName(link);
            var tempDir = Path.Combine(parentTempDir, linkFileName);

            var tempLink = Path.GetTempFileName();
            LINK.Pack(tempDir, tempLink);
            var patchTarget = Path.Combine(item.ContainingFolder, linkFileName);
            filesToPatch.Add(new FileToPatch(patchTarget, tempLink, FilePatchOptions.DeleteSourceWhenDone));
        }

        Directory.Delete(parentTempDir, true);
    }
}
