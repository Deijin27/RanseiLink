using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class IconInstSPatchBuilder(ModInfo mod) : IMiscItemPatchBuilder
{
    public MetaMiscItemId Id => MetaMiscItemId.IconInstS;

    private readonly string _graphicsProviderFolder = Constants.DefaultDataFolder(mod.GameCode);

    private static readonly CellImageSettings _settings = new(
        Prt: PositionRelativeTo.MinCell,
        Debug: false
        );

    public static CellImageSettings Settings => _settings;

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, MiscConstants gInfo, MiscItem miscItem, string pngFile)
    {
        var item = (BuildingIconSmallMiscItem)miscItem;

        var parentTempDir = FileUtil.GetTemporaryDirectory();

        var containingFolder = Path.Combine(_graphicsProviderFolder, item.ContainingFolder);
        using var image = ImageUtil.LoadPngBetterError(pngFile);
        var images = new List<Image<Rgba32>>();

        // Get the link files. If we supported adding additional files
        // this would need to work differently. I assume all the ncers
        // are similar to identical, so it should be do-able.
        var rx = new Regex(@"03_05_parts_shisetsuicon_s_(\d\d)\.G2DR");
        var linkFiles = Directory.GetFiles(containingFolder, "*.G2DR");
        var workingPalette = new PaletteCollection(1, TexFormat.Pltt256, true);
        string? nclrDir = null;

        if (linkFiles.Length == 0)
        {
            throw new Exception($"no link files found at '{containingFolder}'");
        }

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

            const NcgrSlot ncgrSlot = NcgrSlot.Slot3;
            var ncgr = G2DR.LoadPixelsFromFolder(tempDir, ncgrSlot);
            const int width = 32;
            const int height = 32;

            var ncer = G2DR.LoadCellFromFolder(tempDir);

            // Crop the part of the image for this link
            using var subImage = image.Clone(g =>
                g.Crop(new Rectangle(0, (id * height), width, height)));

            var workingPixels = new List<byte>();
            CellImageUtil.SharedPaletteMultiClusterFromImage(
                image: subImage,
                ncer.Clusters.Clusters,
                workingPixels,
                workingPalette,
                null,
                ncer.Clusters.BlockSize,
                tiled: ncgr.Pixels.IsTiled,
                format: ncgr.Pixels.Format,
                settings: _settings
                );

            var pixels = workingPixels.ToArray();

            if (ncgr.Pixels.Data.Length != pixels.Length)
            {
                throw new Exception("Image was different size when processing IconInstS");
            }
            ncgr.Pixels.Data = pixels;
            G2DR.SavePixelsToFolder(tempDir, ncgr, ncgrSlot);

            if (id == 0)
            {
                // this is the one where the palette is stored
                // so we store a reference to it. The later, when we have calculated the
                // full palette, we can write it to the file
                nclrDir = tempDir;
            }

        }

        if (nclrDir == null)
        {
            throw new Exception("There was no link 0 file for IconInstS");
        }

        // Save the palette
        // if the palette is too big this is an error
        // if the palette is too small it will be scaled to the right 
        // length via the Array.Copy
        var nclr = G2DR.LoadPaletteFromFolder(nclrDir);
        var newPalette = PaletteUtil.From32bitColors(workingPalette[0]);
        if (newPalette.Length > nclr.Palettes.Palette.Length)
        {
            throw new InvalidPaletteException($"Palette is bigger than allowed for in IconInstS ({newPalette.Length} vs {nclr.Palettes.Palette.Length})");
        }
        Array.Copy(newPalette, nclr.Palettes.Palette, newPalette.Length);
        G2DR.SavePaletteOnlyToFolder(nclrDir, nclr);

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
