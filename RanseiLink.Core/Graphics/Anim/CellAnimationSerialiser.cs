using FluentResults;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
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
        ExportAnimationXml(anim.Nanr, anim.Ncer, anim.Ncgr, anim.Nclr, outputFolder, width, height, settings, fmt, background);
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

            var nanr = ImportAnimationXml(res, dir, anim.Ncer, anim.Ncgr, anim.Nclr, width, height, settings);
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

    /*
public static void DeserialiseFromScratch(string inputFolder, string outputBgLinkFile, string? outputAnimLinkFile = null)
{
   // technically we will always have the background file default to work off of
   var tempBg = FileUtil.GetTemporaryDirectory();
   try
   {
       var ncgr = new NCGR();
       var nclr = new NCLR();

       // TODO: set up the ncgr and nclr properly
       var (width, height) = DeserialiseBackground(inputFolder, ncgr, nclr);
       // G2DR.SaveScreenToFile(tempBg, nscr, ncgr, nclr, NcgrSlot.Slot1);
   }
   finally
   {
       Directory.Delete(tempBg, true);
   }

   // load animation
   if (outputAnimLinkFile != null)
   {
       var tempAnim = FileUtil.GetTemporaryDirectory();
       try
       {
           var ncer = new NCER();
           var ncgr = new NCGR();
           var nclr = new NCLR();
           // TODO: set up the nanr, ncgr and nclr properly
           var nanr = DeserialiseAnimationXml(Path.Combine(inputFolder, "animation.xml"), ncer, ncgr, nclr);
           // G2DR.SaveAnimToFile(tempAnim, nanr, ncer, ncgr, nclr, NcgrSlot.Slot1);
       }
       finally
       {
           Directory.Delete(tempAnim, true);
       }
   }
}
*/

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

    public static void ExportAnimationXml(NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, int width, int height, CellImageSettings settings, RLAnimationFormat fmt, string? background)
    {
        var dims = CellImageUtil.InferDimensions(null, width, height, settings);

        // save animations
        var anims = ExportNanr(nanr);

        // save cells
        List<RLAnimationResource.ClusterInfo> clusters;
        if (fmt == RLAnimationFormat.OneImagePerCluster)
        {
            clusters = ExportOneImagePerCluster(ncer, ncgr, nclr, outputFolder, width, height, settings, dims);
        }
        else if (fmt == RLAnimationFormat.OneImagePerCell)
        {
            clusters = ExportOneImagePerCell(ncer, ncgr, nclr, outputFolder, dims);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(fmt), fmt, null);
        }
        
        var res = new RLAnimationResource(fmt, background);
        res.Animations.AddRange(anims);
        res.Clusters.AddRange(clusters);

        var doc = res.Serialise();
        doc.Save(Path.Combine(outputFolder, "animation.xml"));
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

    private static List<RLAnimationResource.ClusterInfo> ExportOneImagePerCluster(NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, int width, int height, CellImageSettings settings, ClusterDimensions dims)
    {
        if (width <= 0 || height <= 0)
        {
            throw new Exception($"With format {RLAnimationFormat.OneImagePerCluster} width and height must be specified");
        }
        var images = NitroImageUtil.NcerToMultipleImages(ncer, ncgr, nclr, settings, width, height);

        var clusters = new List<RLAnimationResource.ClusterInfo>();

        for (int clusterId = 0; clusterId < ncer.Clusters.Clusters.Count; clusterId++)
        {
            // save cluster image
            var clusterImage = images[clusterId];
            var fileName = $"{clusterId.ToString().PadLeft(4, '0')}.png";
            clusterImage.SaveAsPng(Path.Combine(outputFolder, fileName));
            clusterImage.Dispose();

            // save cluster data
            var cluster = ncer.Clusters.Clusters[clusterId];
            var clusterData = new RLAnimationResource.ClusterInfo(ClusterToString(clusterId)) { File = fileName };
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
                    Palette = cell.IndexPalette,
                    DoubleSize = cell.DoubleSize,
                    Height = cell.Height,
                    Width = cell.Width,
                };
                clusterData.Cells.Add(cellData);
            }
        }

        return clusters;
    }

    

    private static List<RLAnimationResource.ClusterInfo> ExportOneImagePerCell(NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, ClusterDimensions dims)
    {
        var distinctImages = new List<(int TileOffset, int IndexPalette, byte[] Hash, string FileName)>();
        var imageGroups = NitroImageUtil.NcerToMultipleImageGroups(ncer, ncgr, nclr);

        var clusters = new List<RLAnimationResource.ClusterInfo>();

        for (int clusterId = 0; clusterId < ncer.Clusters.Clusters.Count; clusterId++)
        {
            // prepare image folders
            string folderName = clusterId.ToString().PadLeft(4, '0');
            //Directory.CreateDirectory(Path.Combine(outputFolder, folderName));
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
                var filePath = Path.Combine(outputFolder, fileName);
                cellImage.SaveAsPng(filePath);

                // only save distinct images
                var sha = FileUtil.Sha256File(filePath);
                bool found = false;
                foreach (var tup in distinctImages)
                {
                    // tup.TileOffset == cell.TileOffset && tup.IndexPalette == cell.IndexPalette && 
                    if (tup.Hash.SequenceEqual(sha))
                    {
                        found = true;
                        fileName = tup.FileName;
                        File.Delete(filePath);
                    }
                }
                if (!found)
                {
                    distinctImages.Add((cell.TileOffset, cell.IndexPalette, sha, fileName));
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

        return clusters;
    }



    /// <summary>
    /// Warning: will throw an exception on failure
    /// </summary>
    public static NANR ImportAnimationXml(RLAnimationResource res, string dir, NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, CellImageSettings settings)
    {
        var dims = CellImageUtil.InferDimensions(null, width, height, settings);

        // clear it ready for adding our own cell banks
        ncer.Clusters.Clusters.Clear();
        ncer.Labels.Names.Clear();

        // Generate a map of cluster name to ID to be used to import the animations
        var nameToClusterId = GenerateClusterMap(res);

        var fmt = res.Format;
        if (fmt == RLAnimationFormat.OneImagePerCluster)
        {
            ImportOneImagePerCluster(ncer, ncgr, nclr, settings, dims, res, dir);
        }
        else if (fmt == RLAnimationFormat.OneImagePerCell)
        {
            ImportOneImagePerCell(ncer, ncgr, nclr, dims, res, dir);
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
                if (!nameToClusterId.TryGetValue(frame.Cluster, out var bankId))
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

    private static Dictionary<string, ushort> GenerateClusterMap(RLAnimationResource res)
    {
        var nameToClusterId = new Dictionary<string, ushort>();
        for (int i = 0; i < res.Clusters.Count; i++)
        {
            var clusterInfo = res.Clusters[i];
            if (nameToClusterId.ContainsKey(clusterInfo.Name))
            {
                throw new Exception($"More than one cluster has the same name {clusterInfo.Name}");
            }
            nameToClusterId.Add(clusterInfo.Name, (ushort)i);
        }
        return nameToClusterId;
    }

    private static void ImportOneImagePerCell(NCER ncer, NCGR ncgr, NCLR nclr, ClusterDimensions dims, RLAnimationResource res, string dir)
    {
        var imageGroups = new List<IReadOnlyList<Image<Rgba32>>>();
        foreach (var clusterInfo in res.Clusters)
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
                    IndexPalette = (byte)cellInfo.Palette,
                    Depth = nclr.Palettes.Format == TexFormat.Pltt16 ? BitDepth.e4Bit : BitDepth.e8Bit
                };
                bank.Add(cell);
                if (string.IsNullOrEmpty(cellInfo.File))
                {
                    throw new Exception($"Missing required attribute 'file' on cell for format {RLAnimationFormat.OneImagePerCell}");
                }
                var imgPath = Path.Combine(dir, FileUtil.NormalizePath(cellInfo.File));
                var img = ImageUtil.LoadPngBetterError(imgPath);
                images.Add(img);
                cell.Width = img.Width;
                cell.Height = img.Height;
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

    private static void ImportOneImagePerCluster(NCER ncer, NCGR ncgr, NCLR nclr, CellImageSettings settings, ClusterDimensions dims, RLAnimationResource res, string dir)
    {
        var fmt = RLAnimationFormat.OneImagePerCluster;
        List<Image<Rgba32>> images = [];

        foreach (var clusterInfo in res.Clusters)
        {
            var bank = new Cluster();
            ncer.Clusters.Clusters.Add(bank);

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
                    IndexPalette = (byte)cellInfo.Palette,
                    Width = cellInfo.Width,
                    Height = cellInfo.Height,
                };
                CalculateShapeAndScale(cell);
                bank.Add(cell);
            }

            // image path is relative to the location of the xml file
            if (string.IsNullOrEmpty(clusterInfo.File))
            {
                throw new Exception($"Missing required attribute 'file' on cell group for format {fmt}");
            }
            var imgPath = Path.Combine(dir, FileUtil.NormalizePath(clusterInfo.File));
            images.Add(ImageUtil.LoadPngBetterError(imgPath));

            bank.EstimateMinMaxValues();
            // TODO: cluster.ReadOnlyCellInfo = ?;
        }

        // import the image data
        NitroImageUtil.NcerFromMultipleImages(ncer, ncgr, nclr, images, settings);

        // dispose of the images as we don't need them anymore
        foreach (var image in images)
        {
            image.Dispose();
        }
    }

}
