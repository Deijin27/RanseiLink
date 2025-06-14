using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using SixLabors.ImageSharp;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class NSCRMiscItem : G2DRMiscItem
{
    public NSCRMiscItem(MetaMiscItemId metaId, int id, XElement element) : base(metaId, id, element)
    {

    }

    public override void ProcessExportedFiles(string defaultDataFolder, MiscConstants gInfo)
    {
        var outFolder = Path.Combine(defaultDataFolder, LinkFolder);
        LINK.Unpack(Path.Combine(defaultDataFolder, Link), outFolder);

        var (ncgr, nclr) = G2DR.LoadImgFromFolder(outFolder);
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(defaultDataFolder, PngFile));
    }

    public override void GetFilesToPatch(GraphicsPatchContext context, MiscConstants gInfo, string pngFile)
    {
        var tempDir = FileUtil.GetTemporaryDirectory();

        FileUtil.CopyFilesRecursively(Path.Combine(context.DefaultDataFolder, LinkFolder), tempDir);
        File.Delete(Path.Combine(tempDir, Path.GetFileName(PngFile)));

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
        context.FilesToPatch.Add(new FileToPatch(Link, tempLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));

    }
}
