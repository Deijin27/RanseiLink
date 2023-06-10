using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Graphics;

public class SpriteImageInfo
{
    public SpriteImageInfo(byte[] pixels, Rgba32[] palette, int width, int height)
    {
        Pixels = pixels;
        Palette = palette;
        Width = width;
        Height = height;
    }

    public byte[] Pixels { get; set; } 
    public Rgba32[] Palette { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
