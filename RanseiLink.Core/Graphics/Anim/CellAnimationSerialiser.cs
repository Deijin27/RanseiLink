using FluentResults;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Xml.Linq;

namespace RanseiLink.Core.Graphics;

public enum RLAnimationFormat
{
    OneImagePerCell,
    OneImagePerCluster
}

public static class CellAnimationSerialiser
{

    /// <summary>
    /// If format is <see cref="RLAnimationFormat.OneImagePerCluster"/> then width/height are unnecessary
    /// </summary>
    public static void ExportAnimationOnly(CellImageSettings settings, string outputFolder, string animLinkFile, int width, int height, RLAnimationFormat fmt, string? background)
    {
        var anim = G2DR.LoadAnimFromFile(animLinkFile);
        var exportData = ExportAnimationXml(anim.Nanr, anim.Ncer, anim.Ncgr, anim.Nclr, width, height, settings, fmt, background);
        var doc = exportData.Res.Serialise();
        doc.Save(Path.Combine(outputFolder, "animation.xml"));

        foreach (var (fileName, img) in exportData.Images)
        {
            img.SaveAsPng(Path.Combine(outputFolder, fileName));
            img.Dispose();
        }
    }

    public static Result ImportAnimation(CellImageSettings settings, string animLinkFile, string animationXml, int width, int height, string outputAnimLinkFile, RLAnimationResource? res = null)
    {
        var tempAnim = FileUtil.GetTemporaryDirectory();
        try
        {
            res ??= new RLAnimationResource(XDocument.Load(animationXml));
            var dir = Path.GetDirectoryName(animationXml)!;
            LINK.Unpack(animLinkFile, tempAnim);
            var anim = G2DR.LoadAnimImgFromFolder(tempAnim);

            var valid = ValidateAnim(res, anim.Nanr);
            if (valid.IsFailed)
            {
                return valid;
            }

            var images = LoadImages(dir, res);

            var data = new FullExportData(res, images);

            var nanr = ImportAnimationXml(data, anim.Ncer, anim.Ncgr, anim.Nclr, width, height, settings);
            G2DR.SaveAnimImgToFolder(tempAnim, nanr, anim.Ncer, anim.Ncgr, anim.Nclr, NcgrSlot.Infer);
            LINK.Pack(tempAnim, outputAnimLinkFile);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to import animation '{animationXml}'. Reason: {ex}");
        }
        finally
        {
            Directory.Delete(tempAnim, true);
        }
    }

    private static Dictionary<string, Image<Rgba32>> LoadImages(string dir, RLAnimationResource res)
    {
        var images = new Dictionary<string, Image<Rgba32>>(StringComparer.OrdinalIgnoreCase);

        var fileNames = new HashSet<string>();

        foreach (var cluster in res.Clusters)
        {
            fileNames.Add(cluster.File ?? "");

            foreach (var cell in cluster.Cells)
            {
                fileNames.Add(cell.File ?? "");
                if (string.IsNullOrEmpty(cell.File) && res.Format == RLAnimationFormat.OneImagePerCell)
                {
                    throw new Exception($"Missing required attribute 'file' on cell for format {RLAnimationFormat.OneImagePerCell}");
                }
            }
        }

        foreach (var fileName in fileNames)
        {
            Image<Rgba32> image;
            if (fileName == "")
            {
                image = new Image<Rgba32>(1, 1);
            }
            else
            {
                image = ImageUtil.LoadPngBetterError(Path.Combine(dir, FileUtil.NormalizePath(fileName)));
            }

            images[fileName] = image;
        }

        return images;
    }

    private static Result ValidateAnim(RLAnimationResource res, NANR anim)
    {
        // Animation is corrupt if you use a different amount of animations
        // to what were originally in the slot.
        // This is very annoying, why does it even matter??????

        var requiredAnimCount = anim.AnimationBanks.Banks.Count;
        if (res.Animations.Count > requiredAnimCount)
        {
            return Result.Fail($"This particular slot can only have up to {requiredAnimCount} animations.");
        }

        // if it's less, we can add blank ones to the resource
        if (res.Animations.Count < requiredAnimCount)
        {
            var diff = requiredAnimCount - res.Animations.Count;

            for (int i = 0; i < diff; i++)
            {
                // create a unique name for the anim
                string name;
                int n = 0;
                do
                {
                    name = $"CellAnime{n++}";
                }
                while (res.Animations.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)));

                res.Animations.Add(new RLAnimationResource.Anim(name, []));
            }
        }

        return Result.Ok();
    }


    public static void Export(CellImageSettings settings, RLAnimationFormat fmt, string outputFolder, string bgLinkFile, string? animLinkFile = null)
    {
        const string backgroundFile = "background.png";
        var bg = G2DR.LoadImgFromFile(bgLinkFile);
        var (width, height) = ExportBackground(bg.Ncgr, bg.Nclr, outputFolder, backgroundFile);

        if (animLinkFile != null)
        {
            ExportAnimationOnly(settings, outputFolder, animLinkFile, width, height, fmt: fmt, backgroundFile);
        }
    }

    public static Result ImportAnimAndBackground(AnimationTypeId type, CellImageSettings settings,
        string animationXml, string animLinkFile, string outputAnimLinkFile,
        string bgLinkFile, string outputBgLinkFile)
    {
        try
        {
            // pre-load animation resource to locate background file
            RLAnimationResource res = new RLAnimationResource(XDocument.Load(animationXml));
            if (string.IsNullOrEmpty(res.Background))
            {
                return Result.Fail($"Required attribute 'background' not specified in animation file '{bgLinkFile}'");
            }

            // load background
            var dir = Path.GetDirectoryName(animationXml)!;
            var backgroundFile = Path.Combine(dir, FileUtil.NormalizePath(res.Background));

            var bgResult = ImportBackground(type, backgroundFile, bgLinkFile, outputBgLinkFile);
            if (bgResult.IsFailed)
            {
                return bgResult.ToResult();
            }
            var (width, height) = bgResult.Value;

            // finally load the animation using the dimensions obtained from the background
            return ImportAnimation(settings, animLinkFile, animationXml, width, height, outputAnimLinkFile, res);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.ToString());
        }
    }

    /// <summary>
    /// Imports a background file
    /// </summary>
    /// <param name="bgImage">Absolute path of background image file</param>
    /// <param name="bgLinkFile">Absolute path of current background link file to inherit information from</param>
    /// <param name="outputBgLinkFile">Absolute path to put the output background link file</param>
    /// <returns>Width and height of the background that was imported</returns>
    public static Result<(int width, int height)> ImportBackground(AnimationTypeId type, string bgImage, string bgLinkFile, string outputBgLinkFile)
    {
        var tempBg = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(bgLinkFile, tempBg);
            var bg = G2DR.LoadImgFromFolder(tempBg);
            var res = ImportBackground(type, bgImage, bg.Ncgr, bg.Nclr);
            G2DR.SaveImgToFolder(tempBg, bg.Ncgr, bg.Nclr, NcgrSlot.Infer);
            LINK.Pack(tempBg, outputBgLinkFile);
            return Result.Ok(res);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to import background file '{bgImage}'. Reason: {ex}");
        }
        finally
        {
            Directory.Delete(tempBg, true);
        }
    }

    public static (int width, int height) ExportBackground(NCGR ncgr, NCLR nclr, string outputFolder, string outputFileName)
    {
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(outputFolder, outputFileName));

        return (image.Width, image.Height);
    }

    public static (int width, int height) ImportBackground(AnimationTypeId type, string inputFile, NCGR ncgr, NCLR nclr)
    {
        using var image = ImageUtil.LoadPngBetterError(inputFile);

        if (type == AnimationTypeId.Castlemap)
        {
            // we pre-pad the palette because the first 16 are used for something
            // idk why, but if you fill it up the colors of portions of the image are messed up.
            var palette = new Palette(ncgr.Pixels.Format, true);
            for (int i = 0; i < 15; i++)
            {
                // adding transparent because a different color may be used in the image
                // this ensures these slots remain unused
                palette.Add(Color.Transparent);
            }
            var pixels = ImageUtil.SharedPalettePixelsFromImage(image, palette, ncgr.Pixels.IsTiled, ncgr.Pixels.Format, color0ToTransparent: true);
            ncgr.Pixels.Data = pixels;
            ncgr.Pixels.TilesPerRow = (short)(image.Width / 8);
            ncgr.Pixels.TilesPerColumn = (short)(image.Height / 8);

            // now the transparents will be interpreted as black so it's probably fine to not swap them out
            var newPalette = PaletteUtil.From32bitColors(palette);
            if (newPalette.Length > nclr.Palettes.Palette.Length)
            {
                // this should not be hit because it should be filtered out by the palette simplifier
                throw new InvalidPaletteException($"Palette length exceeds current palette when importing image {newPalette.Length} vs {nclr.Palettes.Palette.Length}");
            }
            newPalette.CopyTo(nclr.Palettes.Palette, 0);
        }
        else
        {
            NitroImageUtil.NcgrFromImage(ncgr, nclr, image);
        }
        
        return (image.Width, image.Height);
    }

    public static FullExportData ExportAnimationXml(NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, CellImageSettings settings, RLAnimationFormat fmt, string? background)
    {
        var dims = CellImageUtil.InferDimensions(null, width, height, settings);

        // save animations
        var anims = ExportNanr(nanr);

        // save cells
        ExportData exportData;
        if (fmt == RLAnimationFormat.OneImagePerCluster)
        {
            exportData = ExportOneImagePerCluster(ncer, ncgr, nclr, width, height, settings, dims);
        }
        else if (fmt == RLAnimationFormat.OneImagePerCell)
        {
            exportData = ExportOneImagePerCell(ncer, ncgr, nclr, dims);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(fmt), fmt, null);
        }


        var res = new RLAnimationResource(fmt, background);
        res.Animations.AddRange(anims);
        res.Clusters.AddRange(exportData.Clusters);

        return new(res, exportData.Images);
    }

    private static List<RLAnimationResource.Anim> ExportNanr(NANR nanr)
    {
        var anims = new List<RLAnimationResource.Anim>();
        for (int i = 0; i < nanr.AnimationBanks.Banks.Count; i++)
        {
            var anim = nanr.AnimationBanks.Banks[i];
            var name = nanr.Labels.Names[i];
            anims.Add(new RLAnimationResource.Anim(name, anim.Frames.Select(x => new RLAnimationResource.AnimFrame(ClusterToString(x.Cluster), x.Duration)).ToList()));
        }
        return anims;
    }

    private static string ClusterToString(int clusterId)
    {
        return $"cluster_{clusterId}";
    }

    public record FullExportData(RLAnimationResource Res, Dictionary<string, Image<Rgba32>> Images);
    public record ExportData(List<RLAnimationResource.ClusterInfo> Clusters, Dictionary<string, Image<Rgba32>> Images);

    private static ExportData ExportOneImagePerCluster(NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, CellImageSettings settings, ClusterDimensions dims)
    {
        if (width <= 0 || height <= 0)
        {
            throw new Exception($"With format {RLAnimationFormat.OneImagePerCluster} width and height must be specified");
        }
        var imagesList = NitroImageUtil.NcerToMultipleImages(ncer, ncgr, nclr, settings, width, height);

        var clusters = new List<RLAnimationResource.ClusterInfo>();
        var images = new Dictionary<string, Image<Rgba32>>();

        for (int clusterId = 0; clusterId < ncer.Clusters.Clusters.Count; clusterId++)
        {
            // save cluster image
            var clusterImage = imagesList[clusterId];
            var fileName = $"{clusterId.ToString().PadLeft(4, '0')}.png";
            images[fileName] = clusterImage;

            // save cluster data
            var cluster = ncer.Clusters.Clusters[clusterId];
            var clusterData = new RLAnimationResource.ClusterInfo(ClusterToString(clusterId))
            {
                File = fileName,
                Palette = cluster.Count > 0 ? cluster[0].IndexPalette : (byte)0
            };
            clusters.Add(clusterData);

            foreach (var cell in cluster)
            {
                // save cell data
                var cellData = new RLAnimationResource.CellInfo()
                {
                    X = cell.XOffset + dims.XShift,
                    Y = cell.YOffset + dims.YShift,
                    FlipX = cell.FlipX,
                    FlipY = cell.FlipY,
                    Palette = 0, // palette is stored on cluster
                    DoubleSize = cell.DoubleSize,
                    Height = cell.Height,
                    Width = cell.Width,
                };
                if (clusterData.Palette != cell.IndexPalette)
                {
                    throw new Exception($"Cannot export with format {nameof(RLAnimationFormat.OneImagePerCluster)} because not all cells in cluster {clusterId} use the same palette");
                }
                clusterData.Cells.Add(cellData);
            }
        }

        return new(clusters, images);
    }

    private static ExportData ExportOneImagePerCell(NCER ncer, NCGR ncgr, NCLR nclr, ClusterDimensions dims)
    {
        var distinctImages = new List<(int TileOffset, int IndexPalette, byte[] Hash, string FileName, Image<Rgba32> Image)>();
        var imageGroups = NitroImageUtil.NcerToMultipleImageGroups(ncer, ncgr, nclr);

        var clusters = new List<RLAnimationResource.ClusterInfo>();
        var images = new Dictionary<string, Image<Rgba32>>();

        for (int clusterId = 0; clusterId < ncer.Clusters.Clusters.Count; clusterId++)
        {
            // prepare image folders
            string folderName = clusterId.ToString().PadLeft(4, '0');
            var group = imageGroups[clusterId];

            // save bank data
            var cluster = ncer.Clusters.Clusters[clusterId];
            var clusterData = new RLAnimationResource.ClusterInfo(ClusterToString(clusterId));
            clusters.Add(clusterData);

            for (int cellId = 0; cellId < cluster.Count; cellId++)
            {
                var cell = cluster[cellId];

                // save cell image
                var cellImage = group[cellId];
                var fileName = $"{folderName}_{cellId.ToString().PadLeft(4, '0')}.png";
                

                // only save distinct images
                var sha = FileUtil.Sha256Image(cellImage);
                bool found = false;
                foreach (var tup in distinctImages)
                {
                    // tup.TileOffset == cell.TileOffset && tup.IndexPalette == cell.IndexPalette && 
                    if (tup.Hash.SequenceEqual(sha))
                    {
                        found = true;
                        fileName = tup.FileName;
                    }
                }
                if (!found)
                {
                    distinctImages.Add((cell.TileOffset, cell.IndexPalette, sha, fileName, cellImage));
                }
                // save cell data

                var cellData = new RLAnimationResource.CellInfo()
                {
                    File = fileName,
                    X = cell.XOffset + dims.XShift,
                    Y = cell.YOffset + dims.YShift,
                    FlipX = cell.FlipX,
                    FlipY = cell.FlipY,
                    Palette = cell.IndexPalette,
                    DoubleSize = cell.DoubleSize,
                };
                clusterData.Cells.Add(cellData);
            }
        }

        return new(clusters, distinctImages.ToDictionary(x => x.FileName, x => x.Image));
    }

    /// <summary>
    /// Warning: will throw an exception on failure
    /// </summary>
    public static NANR ImportAnimationXml(FullExportData data, NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, CellImageSettings settings)
    {
        var res = data.Res;
        var dims = CellImageUtil.InferDimensions(null, width, height, settings);

        // clear it ready for adding our own cell banks
        ncer.Clusters.Clusters.Clear();
        ncer.Labels.Names.Clear();

        var requiresAdditionalEmptyCluster = res.Animations.SelectMany(x => x.Frames).Any(x => string.IsNullOrEmpty(x.Cluster));
        if (requiresAdditionalEmptyCluster)
        {
            ncer.Clusters.Clusters.Add(new Cluster());
        }

        // Generate a map of cluster name to ID to be used to import the animations
        var nameToClusterId = GenerateClusterMap(res, requiresAdditionalEmptyCluster);

        var innerData = new ExportData(res.Clusters, data.Images);

        var fmt = res.Format;
        if (fmt == RLAnimationFormat.OneImagePerCluster)
        {
            ImportOneImagePerCluster(ncer, ncgr, nclr, settings, dims, innerData);
        }
        else if (fmt == RLAnimationFormat.OneImagePerCell)
        {
            ImportOneImagePerCell(ncer, ncgr, nclr, dims, innerData);
        }
        else
        {
            throw new Exception($"Invalid serialisation format {res.Format}");
        }

        NANR nanr = ImportNanr(res, nameToClusterId);

        // for some reason it stores animation names in here as well as the LABL section in NANR
        ncer.Labels.Names.AddRange(nanr.Labels.Names); 

        return nanr;
    }

    private static NANR ImportNanr(RLAnimationResource res, Dictionary<string, ushort> nameToClusterId)
    {
        // load the animations
        var nanr = new NANR();
        foreach (var anim in res.Animations)
        {
            var targetAnim = new ABNK.Anim();
            // TODO: set datatype, unknowns1,2,3
            nanr.AnimationBanks.Banks.Add(targetAnim);
            nanr.Labels.Names.Add(anim.Name);
            foreach (var frame in anim.Frames)
            {
                var targetFrame = new ABNK.Frame();
                targetAnim.Frames.Add(targetFrame);
                if (!nameToClusterId.TryGetValue(frame.Cluster ?? "", out var bankId))
                {
                    throw new Exception($"Animation '{anim.Name}' references cluster of name '{frame.Cluster}' which doesn't exist");
                }
                targetFrame.Cluster = bankId;
                targetFrame.Duration = (ushort)frame.Duration;
            }
        }
        return nanr;
    }

    private static void CalculateShapeAndScale(Cell cell)
    {
        var size = CellImageUtil.GetCellSize(cell.Width, cell.Height);
        if (size == null)
        {
            throw new Exception($"Disallowed cell width and height ({cell.Width}, {cell.Height})");
        }
        cell.Shape = size.Shape;
        cell.Scale = size.Scale;
    }

    private static Dictionary<string, ushort> GenerateClusterMap(RLAnimationResource res, bool requiresAdditionalEmptyCluster)
    {
        var nameToClusterId = new Dictionary<string, ushort>();
        int shift = 0;
        if (requiresAdditionalEmptyCluster)
        {
            nameToClusterId[""] = 0;
            shift = 1;
        }
        for (int i = 0; i < res.Clusters.Count; i++)
        {
            var clusterInfo = res.Clusters[i];
            if (nameToClusterId.ContainsKey(clusterInfo.Name))
            {
                throw new Exception($"More than one cluster has the same name {clusterInfo.Name}");
            }
            nameToClusterId.Add(clusterInfo.Name, (ushort)(i + shift));
        }
        return nameToClusterId;
    }

    private static void ImportOneImagePerCell(NCER ncer, NCGR ncgr, NCLR nclr, ClusterDimensions dims, ExportData data)
    {
        var imageGroups = new List<List<Image<Rgba32>>>();
        foreach (var clusterInfo in data.Clusters)
        {
            List<Image<Rgba32>> images = [];
            imageGroups.Add(images);
            var bank = new Cluster();
            ncer.Clusters.Clusters.Add(bank);

            foreach (var cellInfo in clusterInfo.Cells)
            {
                var cell = new Cell
                {
                    RotateOrScale = cellInfo.DoubleSize ? RotateOrScale.Scale : RotateOrScale.Rotate,
                    XOffset = cellInfo.X - dims.XShift,
                    YOffset = cellInfo.Y - dims.YShift,
                    FlipX = cellInfo.FlipX,
                    FlipY = cellInfo.FlipY,
                    DoubleSize = cellInfo.DoubleSize,
                    IndexPalette = cellInfo.Palette,
                    Depth = nclr.Palettes.Format == TexFormat.Pltt16 ? BitDepth.e4Bit : BitDepth.e8Bit
                };
                bank.Add(cell);
                if (!data.Images.TryGetValue(cellInfo.File ?? string.Empty, out var image))
                {
                    throw new Exception($"Failed to find image '{cellInfo.File}' for cell of cluster '{clusterInfo.Name}' in mode '{nameof(RLAnimationFormat.OneImagePerCell)}'");
                }
                images.Add(image);
                cell.Width = image.Width;
                cell.Height = image.Height;
                CalculateShapeAndScale(cell);
            }

            bank.EstimateMinMaxValues();
            // TODO: cluster.ReadOnlyCellInfo = ?;
        }

        // import the image data
        NitroImageUtil.NcerFromMultipleImageGroups(ncer, ncgr, nclr, imageGroups);

        foreach (var group in imageGroups)
        {
            foreach (var image in group)
            {
                image.Dispose();
            }
        }
    }

    private static void ImportOneImagePerCluster(NCER ncer, NCGR ncgr, NCLR nclr, CellImageSettings settings, ClusterDimensions dims, ExportData data)
    {
        var fmt = RLAnimationFormat.OneImagePerCluster;
        List<Image<Rgba32>> images = [];

        foreach (var clusterInfo in data.Clusters)
        {
            var cluster = new Cluster();
            ncer.Clusters.Clusters.Add(cluster);

            foreach (var cellInfo in clusterInfo.Cells)
            {
                if (cellInfo.Width < 0)
                {
                    throw new Exception($"Missing required attribute 'width' on cell for format {fmt}");
                }
                if (cellInfo.Height < 0)
                {
                    throw new Exception($"Missing required attribute 'height' on cell for format {fmt}");
                }
                var size = CellImageUtil.GetCellSize(cellInfo.Width, cellInfo.Height);
                if (size == null)
                {
                    throw new Exception($"Invalid cell width and height combination ({cellInfo.Width},{cellInfo.Height})");
                }
                var cell = new Cell
                {
                    RotateOrScale = cellInfo.DoubleSize ? RotateOrScale.Scale : RotateOrScale.Rotate,
                    XOffset = cellInfo.X - dims.XShift,
                    YOffset = cellInfo.Y - dims.YShift,
                    FlipX = cellInfo.FlipX,
                    FlipY = cellInfo.FlipY,
                    DoubleSize = cellInfo.DoubleSize,
                    IndexPalette = clusterInfo.Palette,
                    Width = cellInfo.Width,
                    Height = cellInfo.Height,
                };
                CalculateShapeAndScale(cell);
                cluster.Add(cell);
            }

            if (!data.Images.TryGetValue(clusterInfo.File ?? string.Empty, out var image))
            {
                throw new Exception($"Failed to find image '{clusterInfo.File}' for cluster '{clusterInfo.Name}' in mode '{nameof(RLAnimationFormat.OneImagePerCluster)}'");
            }

            images.Add(image);

            cluster.EstimateMinMaxValues();
            // TODO: cluster.ReadOnlyCellInfo = ?;

            if (cluster.Count > 1)
            {
                var firstCellPalette = cluster[0].IndexPalette;
                foreach (var cell in cluster)
                {
                    if (cell.IndexPalette != firstCellPalette)
                    {
                        throw new Exception($"All cells of a cluster must use the same palette for mode '{nameof(RLAnimationFormat.OneImagePerCluster)}'");
                    }
                }
            }
        }

        // import the image data
        NitroImageUtil.NcerFromMultipleImages(ncer, ncgr, nclr, images, settings);

        // dispose of the images as we don't need them anymore
        foreach (var image in images)
        {
            image.Dispose();
        }
    }

    

    private static void SimplifyPalette(string dir, RLAnimationResource res, int paletteSize, string saveDir)
    {
        // I could copy to temp dir, then save to there and move / delete based on if we did simplify
        // OR 
        // Save the files, creating the directory only if needed, then copy later on.

        var imageLookup = LoadImages(dir, res);
        var savedImageFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (res.Format == RLAnimationFormat.OneImagePerCluster)
        {
            var paletteIndexes = new List<byte?>();
            var images = new List<Image<Rgba32>>();
            foreach (var cluster in res.Clusters)
            {
                paletteIndexes.Add(cluster.Palette);
                images.Add(imageLookup[cluster.File ?? ""]);
            }
            var simplified = SimplifyPalettePerCluster(paletteIndexes, paletteSize, images);
            if (simplified)
            {
                for (int i = 0; i < res.Clusters.Count; i++)
                {
                    var cluster = res.Clusters[i];
                    var img = images[i];
                    if (string.IsNullOrEmpty(cluster.File) || savedImageFileNames.Contains(cluster.File))
                    {
                        continue;
                    }
                    var imgPath = Path.Combine(saveDir, FileUtil.NormalizePath(cluster.File));
                    img.SaveAsPng(imgPath);
                }
            }
        }
        else if (res.Format == RLAnimationFormat.OneImagePerCell)
        {
            var palettesIndexes = new List<List<byte>>();
            var imageGroups = new List<List<Image<Rgba32>>>();
            foreach (var cluster in res.Clusters)
            {
                var indexes = new List<byte>();
                var images = new List<Image<Rgba32>>();
                palettesIndexes.Add(indexes);
                imageGroups.Add(images);
                foreach (var cell in cluster.Cells)
                {
                    indexes.Add(cell.Palette);
                    images.Add(imageLookup[cell.File ?? ""]);
                }
            }
            var simplified = SimplifyPalettePerCell(palettesIndexes, paletteSize, imageGroups);
            if (simplified)
            {
                for (int i = 0; i < res.Clusters.Count; i++)
                {
                    var cluster = res.Clusters[i];
                    var imgGroup = imageGroups[i];
                    for (int j = 0; j < cluster.Cells.Count; j++)
                    {
                        var cell = cluster.Cells[j];
                        var img = imgGroup[j];
                        var imgPath = Path.Combine(saveDir, FileUtil.NormalizePath(cell.File!));
                        img.SaveAsPng(imgPath);
                    }
                }
            }
        }
        else
        {
            throw new Exception($"Unknown animation format {res.Format}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paletteIndexes"></param>
    /// <param name="paletteSize"></param>
    /// <param name="images"></param>
    /// <returns>True if any were simplified</returns>
    private static bool SimplifyPalettePerCluster(List<byte?> paletteIndexes, int paletteSize, List<Image<Rgba32>> images)
    {
        bool anySimplified = false;

        var paletteToClusterMap = new Dictionary<byte, List<int>>();
        for (int clusterId = 0; clusterId < paletteIndexes.Count; clusterId++)
        {
            var paletteOpt = paletteIndexes[clusterId];
            if (paletteOpt == null)
            {
                continue;
            }
            var palette = paletteOpt.Value;
            if (!paletteToClusterMap.TryGetValue(palette, out var list))
            {
                list = [];
                paletteToClusterMap[palette] = list;
            }
            list.Add(clusterId);
        }

        foreach (var (palette, clustersIds) in paletteToClusterMap)
        {
            var sharedPaletteImgs = clustersIds.Select(x => images[x]).ToArray();
            var combined = ImageUtil.CombineImagesVertically(sharedPaletteImgs);
            var simplified = ImageSimplifier.SimplifyPalette(combined, paletteSize);
            if (!simplified)
            {
                continue;
            }
            anySimplified = true;
            var cumulativeHeight = 0;
            for (int i = 0; i < clustersIds.Count; i++)
            {
                var clusterId = clustersIds[i];
                var originalImg = sharedPaletteImgs[i]!;
                var newImage = ImageUtil.Crop(combined, new Rectangle(0, cumulativeHeight, originalImg.Width, originalImg.Height));
                cumulativeHeight += originalImg.Height;
                originalImg.Dispose();
                images[clusterId] = newImage;
            }
        }

        return anySimplified;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paletteIndexes"></param>
    /// <param name="paletteSize"></param>
    /// <param name="images"></param>
    /// <returns>True if any were simplified</returns>
    private static bool SimplifyPalettePerCell(List<List<byte>> palettesIndexes, int paletteSize, List<List<Image<Rgba32>>> imageGroups)
    {
        bool anySimplified = false;
        var paletteToClusterMap = new Dictionary<byte, List<(int ClusterId, int CellId)>>();
        for (int clusterId = 0; clusterId < palettesIndexes.Count; clusterId++)
        {
            var cluster = palettesIndexes[clusterId];
            for (int cellId = 0; cellId < cluster.Count; cellId++)
            {
                var palette = cluster[cellId];

                if (!paletteToClusterMap.TryGetValue(palette, out var list))
                {
                    list = [];
                    paletteToClusterMap[palette] = list;
                }
                list.Add((clusterId, cellId));
            }
        }

        foreach (var (palette, ids) in paletteToClusterMap)
        {
            var sharedPaletteImgs = ids.Select(x => imageGroups[x.ClusterId][x.CellId]).ToArray();
            var combined = ImageUtil.CombineImagesVertically(sharedPaletteImgs);
            var simplified = ImageSimplifier.SimplifyPalette(combined, paletteSize);
            if (!simplified)
            {
                continue;
            }
            anySimplified = true;
            var cumulativeHeight = 0;
            for (int i = 0; i < ids.Count; i++)
            {
                var (clusterId, cellId) = ids[i];
                var originalImg = sharedPaletteImgs[i];
                var newImage = ImageUtil.Crop(combined, new Rectangle(0, cumulativeHeight, originalImg.Width, originalImg.Height));
                cumulativeHeight += originalImg.Height;
                originalImg.Dispose();
                imageGroups[clusterId][cellId] = newImage;
            }
        }

        return anySimplified;
    }
}
