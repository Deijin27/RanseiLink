using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Graphics;

public record SpriteImageInfo(
    byte[] Pixels, 
    Rgba32[] Palette, 
    int Width, 
    int Height
    );
