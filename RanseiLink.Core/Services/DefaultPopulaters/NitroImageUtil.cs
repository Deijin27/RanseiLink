using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public static class NitroImageUtil
{
    public static (int width, int height) InferDimsTiled(NCGR ncgr, int tilesize)
    {
        int width, height;
        if (ncgr.Pixels.TilesPerColumn == -1 || ncgr.Pixels.TilesPerRow == -1)
        {
            width = -1;
            height = -1;
        }
        else
        {
            width = ncgr.Pixels.TilesPerRow * tilesize;
            height = ncgr.Pixels.TilesPerColumn * tilesize;
        }
        return (width, height);
    }
}
