using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// For when rendering cells on a predetermined width & height image,
/// this setting allows you to decide if the cell x and y positions
/// should be interpreted as to 
/// be relative to the top-left of the image (as in kuniimage2), 
/// or relative to the centre of the image (as in castlemap)
/// </summary>
public enum PositionRelativeTo
{
    /// <summary>
    /// The top-leftmost cell should be drawn at (0,0), and others move accordingly. It's acceptable to not specify backround width/height for this, it will scale to fit all cells if you don't specify
    /// </summary>
    MinCell,
    /// <summary>
    /// Top-left of the cell should be draw at (x, y) on the image
    /// </summary>
    TopLeft,
    /// <summary>
    /// Top-left of the cell should be drawn at (w/2 + x, h/2 + y) on the image
    /// </summary>
    Centre
}

public record CellImageSettings(
    PositionRelativeTo Prt = PositionRelativeTo.MinCell,
    bool Debug = false
    );

public record ClusterDimensions(int XShift, int YShift, int Width, int Height);

public static class CellImageUtil
{

    public static ClusterDimensions InferDimensions(Cluster? cluster, int width, int height, CellImageSettings settings)
    {
        int xShift = 0;
        int yShift = 0;

        if (settings.Prt == PositionRelativeTo.MinCell)
        {
            if (cluster == null || cluster.Count == 0)
            {
                // if the cluster is empty, we still return cluster dimensoins which are
                // just the width and height with everything else as default
                // The other values will not be used, so it doesn't matter
                // The width and height should be the values passed in.
                // this will lead to a transparent, blank placeholder image to be produced.
                // but default the width and height to non-zero values
                // for the case where the user doesn't provide them to remain valid image
                return new ClusterDimensions(0, 0, Math.Max(width, 1), Math.Max(height, 1));
            }
            int minY = cluster.Min(i => i.YOffset + yShift);
            int minX = cluster.Min(i => i.XOffset + xShift);
            yShift -= minY;
            xShift -= minX;

            if (width <= 0 || height <= 0)
            {
                // scale dimensions to fit cells
                int maxY = cluster.Max(i => i.YOffset + i.Height);
                int maxX = cluster.Max(i => i.XOffset + i.Width);
                width = Math.Max(maxX - minX, width);
                height = Math.Max(maxY - minY, height);
            }
        }
        else if (settings.Prt == PositionRelativeTo.Centre)
        {
            if (width <= 0 || height <= 0)
            {
                throw new Exception($"Width and Height must be specified if using {nameof(PositionRelativeTo)}.{settings.Prt}");
            }
            xShift = width / 2;
            yShift = height / 2;
        }
        else if (settings.Prt == PositionRelativeTo.TopLeft)
        {
            if (width <= 0 || height <= 0)
            {
                throw new Exception($"Width and Height must be specified if using {nameof(PositionRelativeTo)}.{settings.Prt}");
            }
            xShift = 0;
            yShift = 0;
        }

        return new ClusterDimensions(xShift, yShift, width, height);
    }


    public static Image<Rgba32> CellToImage(Cell cell, uint blockSize, MultiPaletteImageInfo imageInfo)
    {
        byte[] cellPixels = GetCellPixels(cell, blockSize, imageInfo.Format, imageInfo.Pixels);

        var cellImg = ImageUtil.SpriteToImage(new SpriteImageInfo(cellPixels, imageInfo.Palette[cell.IndexPalette], cell.Width, cell.Height, imageInfo.IsTiled, imageInfo.Format));

        if (cell.FlipX || cell.FlipY)
        {
            cellImg.Mutate(g =>
            {
                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });
        }

        return cellImg;
    }

    public static byte[] GetCellPixels(Cell cell, uint blockSize, TexFormat format, byte[] allPixels)
    {
        int tileOffset = cell.TileOffset << (byte)blockSize;

        var startByte = tileOffset * 0x20;
        if (format == TexFormat.Pltt16)
        {
            startByte *= 2; // account for compression e.g. pokemon conquest minimaps
        }
        byte[] cellPixels = allPixels.Skip(startByte).Take(cell.Width * cell.Height).ToArray();
        return cellPixels;
    }

    /// <summary>
    /// For a cluster, create one image per cell
    /// </summary>
    public static IReadOnlyList<Image<Rgba32>> SingleClusterToMultipleImages(Cluster cluster, uint blockSize, MultiPaletteImageInfo imageInfo)
    {
        var images = new Image<Rgba32>[cluster.Count];

        for (int i = 0; i < cluster.Count; i++)
        {
            var cell = cluster[i];
            var image = CellToImage(cell, blockSize, imageInfo);
            images[i] = image;
        }
        return images;
    }

    /// <summary>
    /// For a cluster, create one image containing all cells.
    /// The cells are draw at their x,y positions.
    /// </summary>
    public static Image<Rgba32> SingleClusterToImage(Cluster cluster, uint blockSize, MultiPaletteImageInfo imageInfo, CellImageSettings settings)
    {
        var dims = InferDimensions(cluster, imageInfo.Width, imageInfo.Height, settings);

        var graphic = new Image<Rgba32>(dims.Width, dims.Height);
        // for some reason the cells are drawn in reverse
        // while usually cells don't overlap, sometimes they do e.g. in castlemap illusio
        for (int i = cluster.Count - 1; i >= 0; i--)
        {
            Cell cell = cluster[i];
            using (var cellImg = CellToImage(cell, blockSize, imageInfo))
            {
                graphic.Mutate(g =>
                {
                    g.DrawImage(
                        cellImg,
                        new Point(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift),
                        1);
                });
            }
        }

        if (settings.Debug)
        {
            graphic.Mutate(g =>
            {
                int i = 0;
                foreach (var cell in cluster)
                {
                    g.DrawText(i.ToString(), SystemFonts.CreateFont("Arial", 9), Color.Black, new PointF(cell.XOffset + 2 + dims.XShift, cell.YOffset + 2 + dims.YShift));
                    g.Draw(Pens.Solid(Color.Red, 1), new RectangleF(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift, cell.Width - 1, cell.Height - 1));
                    i++;
                }
            });
        }

        return graphic;
    }

    /// <summary>
    /// For a cluster, create one image containing all cells, then save as a png to a file
    /// </summary>
    public static void SingleClusterToPng(string file, Cluster cluster, uint blockSize, MultiPaletteImageInfo imageInfo, CellImageSettings settings)
    {
        using var graphic = SingleClusterToImage(cluster, blockSize, imageInfo, settings);
        graphic.SaveAsPng(file);
    }

    /// <summary>
    /// For a set of cluters, create one image for each cluster containing all of its cells, 
    /// then stack these images vertically into a single image.
    /// </summary>
    public static Image<Rgba32> MultiClusterToImage(IReadOnlyList<Cluster> clusters, uint blockSize, MultiPaletteImageInfo imageInfo, CellImageSettings settings)
    {
        if (clusters.Count == 0)
        {
            throw new Exception("Can't load image with no cell clusters");
        }
        else if (clusters.Count == 1)
        {
            return SingleClusterToImage(clusters[0], blockSize, imageInfo, settings);
        }
        else
        {
            var images = MultiClusterToMultipleImages(clusters, blockSize, imageInfo, settings);
            return ImageUtil.CombineImagesVertically(images);
        }
    }

    /// <summary>
    /// For a set of clusters, create one image for each cluster containing all of its cells.
    /// </summary>
    public static IReadOnlyList<Image<Rgba32>> MultiClusterToMultipleImages(IReadOnlyList<Cluster> clusters, uint blockSize, MultiPaletteImageInfo imageInfo, CellImageSettings settings)
    {
        return clusters.Select(cluster => SingleClusterToImage(cluster, blockSize, imageInfo, settings)).ToList();
    }

    /// <summary>
    /// For a set of clusters, create one image per cell of each cluster.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<Image<Rgba32>>> MultiClusterToMultipleImageGroups(IReadOnlyCollection<Cluster> clusters, uint blockSize, MultiPaletteImageInfo imageInfo)
    {
        return clusters.Select(cluster => SingleClusterToMultipleImages(cluster, blockSize, imageInfo)).ToList();
    }

    /// <summary>
    /// Calculates the pixels, colors, and cell tile offset from an image. Requires some cell parameters, such as FlipX to be pre filled.
    /// </summary>
    public static void CellFromImage(Image<Rgba32> image, Cell cell, uint blockSize, List<byte> workingPixels, PaletteCollection workingPalette,
        bool tiled, TexFormat format)
    {
        // mutate image based on pre-configured cell parameters
        if (cell.FlipX || cell.FlipY)
        {
            image.Mutate(g =>
            {
                if (cell.FlipX)
                    g.Flip(FlipMode.Horizontal);
                if (cell.FlipY)
                    g.Flip(FlipMode.Vertical);
            });
        }

        int byteBlockSize = (byte)blockSize;
        // work out tile offset
        var startByte = workingPixels.Count;
        if (format == TexFormat.Pltt16)
        {
            startByte /= 2; // account for compression
        }
        var tileOffset = startByte / 0x20;
        cell.TileOffset = tileOffset >> byteBlockSize;

        // get pixels from image
        var pixels = ImageUtil.SharedPalettePixelsFromImage(image, workingPalette[cell.IndexPalette], tiled, format, color0ToTransparent: true);
        workingPixels.AddRange(pixels);

        // pad to the correct length
        while (true)
        {
            var nextTileOffset = workingPixels.Count;
            if (format == TexFormat.Pltt16)
            {
                if ((nextTileOffset % 2) != 0)
                {
                    workingPixels.Add(0);
                    continue;
                }
                nextTileOffset /= 2;
            }
            if ((nextTileOffset % 0x20) != 0)
            {
                workingPixels.Add(0);
                continue;
            }
            nextTileOffset /= 0x20;
            if (((nextTileOffset >> byteBlockSize) << byteBlockSize) != nextTileOffset)
            {
                workingPixels.Add(0);
                continue;
            }
            break;
        }
    }

    /// <summary>
    /// Using a provided palette and pixel buffer, import cluster pixel data from a set of images, each image representing one cell of the cluster
    /// </summary>
    public static void SharedSingleClusterFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, Cluster cluster, uint blockSize,
        List<byte> workingPixels, PaletteCollection workingPalette,
        bool tiled, TexFormat format)
    {
        if (images.Count != cluster.Count)
        {
            throw new ArgumentException($"Images did not have the same number of items as cell cluster ({images.Count} vs {cluster.Count})");
        }

        if (cluster.Count == 0)
        {
            return;
        }

        int previousTileOffset = -1;
        for (int i = 0; i < cluster.Count; i++)
        {
            var image = images[i];
            var cell = cluster[i];
            CellFromImage(image, cell, blockSize, workingPixels, workingPalette, tiled, format);
            if (cell.TileOffset == previousTileOffset)
            {
                throw new Exception("Two cells had the same tile offset. We're not doing optimisations yet, so this should never happen");
            }
            previousTileOffset = cell.TileOffset;
        }
    }


    /// <summary>
    /// Import cluster pixel data from a set of images, each image representing one cell of the cluster
    /// </summary>
    public static MultiPaletteImageInfo SingleClusterFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, Cluster cluster, uint blockSize,
        bool tiled, TexFormat format)
    {
        var workingPixels = new List<byte>();
        var workingPalette = new PaletteCollection(cluster, format, true);

        SharedSingleClusterFromMultipleImages(images, cluster, blockSize, workingPixels, workingPalette, tiled, format);

        return new MultiPaletteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette,
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    /// <summary>
    /// Using a provided palette and pixel buffer, import cluster pixel data from a single image where
    /// each cell is read from the x,y positions specified in <paramref name="cluster"/>
    /// </summary>
    public static void SharedSingleClusterFromImage(Image<Rgba32> image, Cluster cluster, uint blockSize,
        List<byte> workingPixels, PaletteCollection workingPalette,
        bool tiled, TexFormat format, CellImageSettings settings)
    {
        if (cluster.Count == 0)
        {
            return;
        }

        var dims = InferDimensions(cluster, image.Width, image.Height, settings);

        for (int i = 0; i < cluster.Count; i++)
        {
            var cell = cluster[i];
            using (var cellImg = ImageUtil.SafeCrop(image, new Rectangle(cell.XOffset + dims.XShift, cell.YOffset + dims.YShift, cell.Width, cell.Height)))
            {
                CellFromImage(cellImg, cell, blockSize, workingPixels, workingPalette, tiled, format);
            }
        }
    }

    /// <summary>
    /// Import cluster pixel data from a single image where
    /// each cell is read from the x,y positions specified in <paramref name="cluster"/>
    /// </summary>
    public static MultiPaletteImageInfo SingleClusterFromImage(Image<Rgba32> image, Cluster cluster, uint blockSize,
        bool tiled, TexFormat format, CellImageSettings settings)
    {
        var workingPixels = new List<byte>();
        var workingPalette = new PaletteCollection(cluster, format, true);
        

        SharedSingleClusterFromImage(image, cluster, blockSize, workingPixels, workingPalette, tiled, format, settings);

        workingPalette.SetColor0(Color.Magenta);
        return new MultiPaletteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette,
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    /// Import cluster pixel data from a single png image file where
    /// each cell is read from the x,y positions specified in <paramref name="cluster"/>
    /// </summary>
    public static MultiPaletteImageInfo SingleClusterFromPng(string file, Cluster cluster, uint blockSize, 
        bool tiled, TexFormat format, CellImageSettings settings)
    {
        if (cluster.Count == 0)
        {
            throw new Exception($"Tried to load png with empty cluster (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return SingleClusterFromImage(image, cluster, blockSize, tiled, format, settings);
    }

    /// <summary>
    /// Import pixel data of multiple clusters from png image file where each cluster is stacked vertically
    /// </summary>
    public static MultiPaletteImageInfo MultiClusterFromPng(string file, IReadOnlyList<Cluster> clusters, uint blockSize, 
        bool tiled, TexFormat format, CellImageSettings settings, int width = -1, int height = -1)
    {
        if (clusters.Any(x => x.Count == 0))
        {
            throw new Exception($"Tried to load png with empty cluster (when loading file '{file}')");
        }

        using var image = ImageUtil.LoadPngBetterError(file);

        return MultiClusterFromImage(image, clusters, blockSize, tiled, format, settings, width, height);
    }

    /// <summary>
    /// Import pixel data of multiple clusters from an image where each cluster is stacked vertically
    /// </summary>
    public static MultiPaletteImageInfo MultiClusterFromImage(Image<Rgba32> image, IReadOnlyList<Cluster> clusters, uint blockSize, 
        bool tiled, TexFormat format, CellImageSettings settings, int width = -1, int height = -1)
    {
        if (clusters.Count == 0)
        {
            throw new Exception("Can't load image with no cell clusters");
        }
        if (clusters.Count == 1)
        {
            return SingleClusterFromImage(image, clusters[0], blockSize, tiled, format, settings);
        }


        var workingPalette = new PaletteCollection(clusters, format, true);
        var workingPixels = new List<byte>();

        SharedPaletteMultiClusterFromImage(image, clusters, workingPixels, workingPalette, blockSize, tiled, format, settings, width, height);
        workingPalette.SetColor0(Color.Magenta);

        return new MultiPaletteImageInfo(workingPixels.ToArray(), workingPalette, width, height, tiled, format);
    }

    /// <summary>
    /// Using a single palette and pixel buffer, mport pixel data of multiple clusters from an image where each cluster is stacked vertically
    /// </summary>
    public static void SharedPaletteMultiClusterFromImage(Image<Rgba32> image, IReadOnlyList<Cluster> clusters, 
        List<byte> workingPixels, PaletteCollection workingPalette, uint blockSize, 
        bool tiled, TexFormat format, CellImageSettings settings, int width = -1, int height = -1)
    {
        if (clusters.Count == 0)
        {
            throw new Exception("Can't load image with no cell clusters");
        }

        if (clusters.Count == 1)
        {
            SharedSingleClusterFromImage(image, clusters[0], blockSize, workingPixels, workingPalette, tiled, format, settings);
            return;
        }

        var cumulativeHeight = 0;
        foreach (var cluster in clusters)
        {
            var dims = InferDimensions(cluster, width, height, settings);
            using (var subImage = image.Clone(g =>
            {
                g.Crop(new Rectangle(0, cumulativeHeight, dims.Width, dims.Height));

            }))
            {
                SharedSingleClusterFromImage(subImage, cluster, blockSize, workingPixels, workingPalette, tiled, format, settings);
            }
            cumulativeHeight += dims.Height;
        }
    }

    /// <summary>
    /// Import cluster pixel data from a set of one image per cluster
    /// </summary>
    public static MultiPaletteImageInfo MultiClusterFromMultipleImages(IReadOnlyList<Image<Rgba32>> images, IReadOnlyList<Cluster> clusters, uint blockSize, 
        bool tiled, TexFormat format, CellImageSettings settings)
    {
        if (clusters.Count == 0)
        {
            throw new ArgumentException("Can't load image with no cell clusters");
        }
        if (clusters.Count != images.Count)
        {
            throw new ArgumentException($"Images did not have the same number of items as clusters ({images.Count} vs {clusters.Count})");
        }

        var workingPalette = new PaletteCollection(clusters, format, true);
        var workingPixels = new List<byte>();
        for (int i = 0; i < clusters.Count; i++)
        {
            var cluster = clusters[i];
            var image = images[i];
            SharedSingleClusterFromImage(image, cluster, blockSize, workingPixels, workingPalette, tiled, format, settings);
        }

        workingPalette.SetColor0(Color.Magenta);

        return new MultiPaletteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette,
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    /// <summary>
    /// Import cluster pixel data from a set of one image per cell.
    /// </summary>
    public static MultiPaletteImageInfo MultiClusterFromMultipleImageGroups(IReadOnlyList<IReadOnlyList<Image<Rgba32>>> imageGroups, IReadOnlyList<Cluster> clusters, uint blockSize,
        bool tiled, TexFormat format)
    {
        if (clusters.Count == 0)
        {
            throw new ArgumentException("Can't load image with no cell clusters");
        }
        if (clusters.Count != imageGroups.Count)
        {
            throw new ArgumentException($"ImageGroups did not have the same number of items as clusters ({imageGroups.Count} vs {clusters.Count})");
        }

        var workingPalette = new PaletteCollection(clusters, format, true);
        var workingPixels = new List<byte>();
        for (int i = 0; i < clusters.Count; i++)
        {
            var cluster = clusters[i];
            var imageGroup = imageGroups[i];
            SharedSingleClusterFromMultipleImages(imageGroup, cluster, blockSize, workingPixels, workingPalette, tiled, format);
        }

        workingPalette.SetColor0(Color.Magenta);

        return new MultiPaletteImageInfo(
            Pixels: workingPixels.ToArray(),
            Palette: workingPalette,
            Width: -1,
            Height: -1,
            IsTiled: tiled,
            Format: format
            );
    }

    /// <summary>
    /// Get the width and height of a cell for a given shape and scale.
    /// </summary>
    public static CellSize GetCellSize(Shape shape, Scale scale)
    {
        if (!__scaleShapeLookup.TryGetValue(shape, out var scaleDict))
        {
            throw new ArgumentException($"Invalid cell shape {shape}");
        }
        if (!scaleDict.TryGetValue(scale, out var size))
        {
            throw new ArgumentException($"Invalid cell scale {scale}");
        }
        return size;
    }

    /// <summary>
    /// Get the shape and scale of a cell for a given width and height.
    /// Returns null if they are not valid sizes.
    /// </summary>
    public static CellSize? GetCellSize(int width, int height)
    {
        if (__widthHeightLookup.TryGetValue(width, out var heightDict) && heightDict.TryGetValue(height, out var size))
        {
            return size;
        }
        return null;
    }

    private static readonly Dictionary<Shape, Dictionary<Scale, CellSize>> __scaleShapeLookup;
    private static readonly Dictionary<int, Dictionary<int, CellSize>> __widthHeightLookup;

    static CellImageUtil()
    {
        ValidCellSizes =
        [
            new CellSize(Shape.Square, Scale.Small, 8, 8),
            new CellSize(Shape.Square, Scale.Medium, 16, 16),
            new CellSize(Shape.Square, Scale.Large, 32, 32),
            new CellSize(Shape.Square, Scale.XLarge, 64, 64),

            new CellSize(Shape.Wide, Scale.Small, 16, 8),
            new CellSize(Shape.Wide, Scale.Medium, 32, 8),
            new CellSize(Shape.Wide, Scale.Large, 32, 16),
            new CellSize(Shape.Wide, Scale.XLarge, 64, 32),

            new CellSize(Shape.Tall, Scale.Small, 8, 16),
            new CellSize(Shape.Tall, Scale.Medium, 8, 32),
            new CellSize(Shape.Tall, Scale.Large, 16, 32),
            new CellSize(Shape.Tall, Scale.XLarge, 32, 64),
        ];

        __scaleShapeLookup = [];
        __widthHeightLookup = [];

        foreach (var size in ValidCellSizes)
        {
            if (!__scaleShapeLookup.TryGetValue(size.Shape, out var scaleDict))
            {
                scaleDict = [];
                __scaleShapeLookup[size.Shape] = scaleDict;
            }
            scaleDict[size.Scale] = size;

            if (!__widthHeightLookup.TryGetValue(size.Width, out var heightDict))
            {
                heightDict = [];
                __widthHeightLookup[size.Width] = heightDict;
            }
            heightDict[size.Height] = size;
        }

    }

    public static CellSize[] ValidCellSizes { get; }
}

public record CellSize(Shape Shape, Scale Scale, int Width, int Height);