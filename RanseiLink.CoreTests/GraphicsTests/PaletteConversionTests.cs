using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.CoreTests.GraphicsTests;

public class PaletteConversionTests
{
    private static readonly Rgba32[] __colorsRgba32 =
    [
        new(96, 248, 16), new(56, 64, 152), new(32, 232, 168) 
    ];

    private static readonly Rgb15[] __colorsRgb15 =
    [
        new(12, 31, 2), new(7, 8, 19), new(4, 29, 21) 
    ];

    private static readonly byte[] __colorsRgb15Compressed =
    [
        0xEC, 0x0B, 0x07, 0x4D, 0xA4, 0x57
    ];

    [Fact]
    public void Convert15To32()
    {
        PaletteUtil.To32bitColors(__colorsRgb15).Should().Equal(__colorsRgba32);
    }

    [Fact]
    public void Convert32To15()
    {
        PaletteUtil.From32bitColors(__colorsRgba32).Should().Equal(__colorsRgb15);
    }

    [Fact]
    public void Compress()
    {
        PaletteUtil.Compress(__colorsRgb15).Should().Equal(__colorsRgb15Compressed);
    }

    [Fact]
    public void Decompress()
    {
        PaletteUtil.Decompress(__colorsRgb15Compressed).Should().Equal(__colorsRgb15);
    }
}
