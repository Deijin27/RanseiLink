using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class NscrPatchBuilder : IMiscItemPatchBuilder
{
    public MetaMiscItemId Id => MetaMiscItemId.NSCR;

    private readonly string _graphicsProviderFolder;
    public NscrPatchBuilder(ModInfo mod)
    {
        _graphicsProviderFolder = Constants.DefaultDataFolder(mod.GameCode);
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, MiscConstants gInfo, MiscItem miscItem, string pngFile)
    {
        var item = (G2DRMiscItem)miscItem;
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
        NitroImageUtil.NcgrImport(ncgr, nclr, pngFile);

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
