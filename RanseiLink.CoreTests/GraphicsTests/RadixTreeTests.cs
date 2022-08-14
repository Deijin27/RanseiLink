using FluentAssertions;
using RanseiLink.Core.Graphics;
using System.IO;
using Xunit;

namespace RanseiLink.CoreTests.GraphicsTests;
public class RadixTreeTests
{
    [Fact]
    public void TestRadixTreeGenerator_Eevee_BTX0_Tex()
    {
        // input names
        var strings = new string[24];
        for (int i = 0; i < 24; i++)
        {
            var s = i.ToString().PadLeft(2, '0');
            strings[i] = $"base_fix_{s}";
        }

        // expected output
        var expected = new byte[] 
        { 
            0x7F, 0x01, 0x00, 0x00,
            0x55, 0x00, 0x02, 0x00,
            0x53, 0x03, 0x16, 0x08,
            0x52, 0x04, 0x0F, 0x04,
            0x51, 0x05, 0x0A, 0x02,
            0x50, 0x06, 0x08, 0x01,
            0x49, 0x07, 0x06, 0x14,
            0x48, 0x01, 0x07, 0x0A,
            0x49, 0x09, 0x08, 0x15,
            0x48, 0x05, 0x09, 0x0B,
            0x50, 0x0B, 0x0D, 0x03,
            0x49, 0x0C, 0x0B, 0x16,
            0x48, 0x04, 0x0C, 0x0C,
            0x49, 0x0E, 0x0D, 0x17,
            0x48, 0x0A, 0x0E, 0x0D,
            0x51, 0x10, 0x13, 0x06,
            0x50, 0x11, 0x12, 0x05,
            0x48, 0x03, 0x11, 0x0E,
            0x48, 0x10, 0x12, 0x0F,
            0x50, 0x14, 0x15, 0x07,
            0x48, 0x0F, 0x14, 0x10,
            0x48, 0x13, 0x15, 0x11,
            0x50, 0x17, 0x18, 0x09,
            0x48, 0x02, 0x17, 0x12,
            0x48, 0x16, 0x18, 0x13
        };

        // generate and write to buffer
        using var memoryStream = new MemoryStream();
        var bw = new BinaryWriter(memoryStream);
        var result = RadixTreeGenerator.Generate(strings);
        foreach (var node in result)
        {
            node.WriteTo(bw);
        }
        var outBytes = memoryStream.ToArray();

        // test result is as expected
        outBytes.Should().Equal(expected);
    }

    [Fact]
    public void TestRadixTreeGenerator_MAP01_BTX0_Tex()
    {
        // input names
        var strings = new string[]
        {
            "map01_00_00",
            "map01_00_00a",
            "map01_00_02a",
            "map01_00_03",
            "map01_00_05",
            "map01_00_05a",
            "map01_00_06",
            "map01_00_06a",
            "map01_00_07a",
        };

        // expected output
        var expected = new byte[]
        {
            0x7F, 0x01, 0x00, 0x00, 
            0x5E, 0x02, 0x06, 0x01, 
            0x55, 0x00, 0x03, 0x00, 
            0x52, 0x04, 0x05, 0x04, 
            0x51, 0x02, 0x04, 0x03, 
            0x51, 0x03, 0x05, 0x06, 
            0x52, 0x07, 0x08, 0x05, 
            0x51, 0x01, 0x07, 0x02, 
            0x51, 0x06, 0x09, 0x07, 
            0x50, 0x08, 0x09, 0x08
        };

        // generate and write to buffer
        using var memoryStream = new MemoryStream();
        var bw = new BinaryWriter(memoryStream);
        var result = RadixTreeGenerator.Generate(strings);
        foreach (var node in result)
        {
            node.WriteTo(bw);
        }
        var outBytes = memoryStream.ToArray();

        // test result is as expected
        outBytes.Should().Equal(expected);
    }
}
