using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using SixLabors.ImageSharp;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class NCERMiscItem : G2DRMiscItem
{
    public static readonly CellImageSettings Settings = new(
        Prt: PositionRelativeTo.MinCell,
        Debug: false
        );

    public NCERMiscItem(MetaMiscItemId metaId, int id, XElement element) : base(metaId, id, element)
    {

    }

    public override void ProcessExportedFiles(PopulateDefaultsContext context, MiscGraphicsInfo gInfo)
    {
        string outFolder = Path.Combine(context.DefaultDataFolder, LinkFolder);
        LINK.Unpack(Path.Combine(context.DefaultDataFolder, Link), outFolder);

        var (ncer, ncgr, nclr) = G2DR.LoadCellImgFromFolder(outFolder, NcgrSlot.Infer);

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Settings);

        image.SaveAsPng(Path.Combine(context.DefaultDataFolder, PngFile));
    }

    public override void GetFilesToPatch(GraphicsPatchContext context, MiscGraphicsInfo gInfo, string pngFile)
    {
        var tempDir = FileUtil.GetTemporaryDirectory();

        FileUtil.CopyFilesRecursively(Path.Combine(context.DefaultDataFolder, LinkFolder), tempDir);
        File.Delete(Path.Combine(tempDir, Path.GetFileName(PngFile)));

        // load up provider data
        var (ncer, ncgr, nclr) = G2DR.LoadCellImgFromFolder(tempDir, NcgrSlot.Infer);

        // load up the png and replace provider data with new image data
        using var image = ImageUtil.LoadPngBetterError(pngFile);
        NitroImageUtil.NcerFromImage(ncer, ncgr, nclr, image, Settings);

        // save the modified provider files
        G2DR.SaveImgToFolder(tempDir, ncgr, nclr, NcgrSlot.Infer);

        // make the link 
        var tempLink = Path.GetTempFileName();
        LINK.Pack(tempDir, tempLink);
        Directory.Delete(tempDir, true);

        context.FilesToPatch.Add(new FileToPatch(Link, tempLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }
}
