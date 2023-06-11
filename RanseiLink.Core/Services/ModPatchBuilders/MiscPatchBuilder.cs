using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Concurrent;
using System.IO;

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

        var newPalette = RawPalette.From32bitColors(imageInfo.Palette);
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

        var newPalette = RawPalette.From32bitColors(imageInfo.Palette);
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
}
