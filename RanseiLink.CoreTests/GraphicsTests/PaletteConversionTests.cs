using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using FluentAssertions;

namespace RanseiLink.CoreTests.GraphicsTests;

public class PaletteConversionTests
{
    private static readonly Rgba32[] _colorsRgba32 = new Rgba32[] 
    { 
        new Rgba32(96, 248, 16), new Rgba32(56, 64, 152), new Rgba32(32, 232, 168) 
    };

    private static readonly Rgb15[] _colorsRgb15 = new Rgb15[] 
    { 
        new Rgb15(12, 31, 2), new Rgb15(7, 8, 19), new Rgb15(4, 29, 21) 
    };

    private static readonly byte[] _colorsRgb15Compressed = new byte[] 
    {
        0xEC, 0x0B, 0x07, 0x4D, 0xA4, 0x57
    };

    [Fact]
    public void Convert15To32()
    {
        RawPalette.To32bitColors(_colorsRgb15).Should().Equal(_colorsRgba32);
    }

    [Fact]
    public void Convert32To15()
    {
        RawPalette.From32bitColors(_colorsRgba32).Should().Equal(_colorsRgb15);
    }

    [Fact]
    public void Compress()
    {
        RawPalette.Compress(_colorsRgb15).Should().Equal(_colorsRgb15Compressed);
    }

    [Fact]
    public void Decompress()
    {
        RawPalette.Decompress(_colorsRgb15Compressed).Should().Equal(_colorsRgb15);
    }
}
