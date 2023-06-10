using SixLabors.ImageSharp;

namespace RanseiLink.Core.Graphics;

public static class PointUtil
{
    public static PointGetter DecidePointGetter(bool isTiled)
    {
        return isTiled ? GetPointTiled8 : GetPoint;
    }

    public static IndexGetter DecideIndexGetter(bool isTiled)
    {
        return isTiled ? GetIndexTiled8 : GetIndex;
    }

    private static Point GetPoint(int index, int imageWidth)
    {
        return new Point(index % imageWidth, index / imageWidth);
    }

    private static int GetIndex(Point point, int imageWidth)
    {
        return point.Y * imageWidth + point.X;
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

    private static int GetIndexTiled(Point point, int imageWidth, int tileSize)
    {
        int pixelsInTile = tileSize * tileSize;
        int tilesInRow = imageWidth / tileSize;

        int x = point.X;
        int y = point.Y;

        // get tile
        int tileX = x / tileSize;
        int tileY = y / tileSize;
        int tileNum = tileY * tilesInRow + tileX;

        // get index in tile
        int xInTile = x % tileSize;
        int yInTile = y % tileSize;
        int indexInTile = yInTile * tileSize + xInTile;

        int index = tileNum * pixelsInTile + indexInTile;

        return index;
    }

    private static Point GetPointTiled8(int index, int imageWidth) => GetPointTiled(index, imageWidth, 8);

    private static int GetIndexTiled8(Point point, int imageWidth) => GetIndexTiled(point, imageWidth, 8);
}

public delegate Point PointGetter(int index, int imageWidth);
public delegate int IndexGetter(Point point, int imageWidth);