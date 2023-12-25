namespace RanseiLink.Core.Graphics;

public record SpriteImageInfo(
    byte[] Pixels, 
    Palette Palette, 
    int Width, 
    int Height,
    bool IsTiled,
    TexFormat Format
    );

public record MultiPaletteImageInfo(
    byte[] Pixels,
    PaletteCollection Palette,
    int Width,
    int Height,
    bool IsTiled,
    TexFormat Format)
{
    public SpriteImageInfo SelectPalette(int paletteId)
    {
        return new SpriteImageInfo(Pixels, Palette[paletteId], Width, Height, IsTiled, Format);
    }
}