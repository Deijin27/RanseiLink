using Xunit;
using FluentAssertions;
using RanseiLink.Core.Graphics;

namespace RanseiLink.CoreTests.GraphicsTests;

public class CharCompressionTests
{
    private static readonly byte[] _compressed = new byte[] { 0xD1, 0x75, 0xCF, 0x73 };
    private static readonly byte[] _uncompressed = new byte[] { 0x1, 0xD, 0x5, 0x7, 0xF, 0xC, 0x3, 0x7 };

    [Fact]
    public void Decompress()
    {
        RawChar.Decompress(_compressed).Should().Equal(_uncompressed);
    }

    [Fact]
    public void Compress()
    {
        RawChar.Compress(_uncompressed).Should().Equal(_compressed);
    }
}
