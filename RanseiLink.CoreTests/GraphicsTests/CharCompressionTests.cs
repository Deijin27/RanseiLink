using RanseiLink.Core.Graphics;

namespace RanseiLink.CoreTests.GraphicsTests;

public class CharCompressionTests
{
    private static readonly byte[] __compressed = [0xD1, 0x75, 0xCF, 0x73];
    private static readonly byte[] __uncompressed = [0x1, 0xD, 0x5, 0x7, 0xF, 0xC, 0x3, 0x7];

    [Fact]
    public void Decompress()
    {
        PixelUtil.Decompress(__compressed).Should().Equal(__uncompressed);
    }

    [Fact]
    public void Compress()
    {
        PixelUtil.Compress(__uncompressed).Should().Equal(__compressed);
    }
}
