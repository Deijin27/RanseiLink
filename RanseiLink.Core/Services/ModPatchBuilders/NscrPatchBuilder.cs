using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class NscrPatchBuilder(ModInfo mod) : IMiscItemPatchBuilder
{
    public MetaMiscItemId Id => MetaMiscItemId.NSCR;

    private readonly string _graphicsProviderFolder = Constants.DefaultDataFolder(mod.GameCode);

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, MiscConstants gInfo, MiscItem miscItem, string pngFile)
    {
        var item = (G2DRMiscItem)miscItem;
        var tempDir = FileUtil.GetTemporaryDirectory();

        FileUtil.CopyFilesRecursively(Path.Combine(_graphicsProviderFolder, item.LinkFolder), tempDir);
        File.Delete(Path.Combine(tempDir, Path.GetFileName(item.PngFile)));

        var (ncgr, nclr) = G2DR.LoadImgFromFolder(tempDir);

        // load up the png and replace provider data with new image data
        using var image = ImageUtil.LoadPngBetterError(pngFile);
        NitroImageUtil.NcgrFromImage(ncgr, nclr, image);

        // save the modified provider files
        G2DR.SaveImgToFolder(tempDir, ncgr, nclr, NcgrSlot.Infer);

        // make the link 
        var tempLink = Path.GetTempFileName();
        LINK.Pack(tempDir, tempLink);
        Directory.Delete(tempDir, true);

        filesToPatch.Add(new FileToPatch(item.Link, tempLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }
}
