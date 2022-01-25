using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace RanseiLink.Core.Graphics;

public static class ImageUtil
{
    private static Point GetPoint(int index, int imageWidth)
    {
        return new Point(index % imageWidth, index / imageWidth);
    }

    private static Point GetPointTiled(int index, int imageWidth, int tileSize)
    {
        int pixelsInTile = tileSize * tileSize;
        int tilesInRow = imageWidth / tileSize;

        int tileNum = index / pixelsInTile;

        // Coordinates of tile in tilespace
        int tileX = tileNum % tilesInRow;
        int tileY = tileNum / tilesInRow;

        // position within tile
        int indexInTile = index % pixelsInTile;


        int x = tileX * tileSize + indexInTile % tileSize;
        int y = tileY * tileSize + indexInTile / tileSize;

        return new Point(x, y);
    }

    public static void SaveAsPngTiled(string file, byte[] pixelMap, Rgba32[] palette, int width, int tileSize)
    {
        SaveAsPng(file, pixelMap, palette, width, (a, b) => GetPointTiled(a, b, tileSize));
    }

    public static void SaveAsPng(string file, byte[] pixelMap, Rgba32[] palette, int width)
    {
        SaveAsPng(file, pixelMap, palette, width, GetPoint);
    }

    private delegate Point PointGetter(int index, int imageWidth);

    private static void SaveAsPng(string file, byte[] pixelMap, Rgba32[] palette, int width, PointGetter pointGetter)
    {
        int height = pixelMap.Length / width;
        using var img = new Image<Rgba32>(width, height);

        for (int i = 0; i < pixelMap.Length; i++)
        {
            Point point = pointGetter(i, width);
            Rgba32 color = palette[pixelMap[i]];
            img[point.X, point.Y] = color;
        }

        img.Save(file, new PngEncoder());
    }
}
