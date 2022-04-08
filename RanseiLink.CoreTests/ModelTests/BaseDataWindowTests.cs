using RanseiLink.Core.Models;
using System;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class BaseDataWindowTests
{
    [Theory]
    [InlineData(1, 0b1)]
    [InlineData(2, 0b11)]
    [InlineData(3, 0b111)]
    [InlineData(4, 0b1111)]
    [InlineData(5, 0b11111)]
    public void GetMaskGivesCorrectResult(int bitCount, uint result)
    {
        Assert.Equal(result, BaseDataWindow.GetMask(bitCount));
    }

    [Fact]
    public void ThrowsExceptionWithIncorrectByteCount()
    {
        byte[] bytes = new byte[8];
        Assert.Throws<ArgumentException>(() => new BaseDataWindow(bytes, 4));
    }

    [Theory]
    [InlineData(1, 5, 1, 0x19u)]
    public void GetUInt32GivesCorrectResult(int index, int bitCount, int offset, uint result)
    {
        byte[] bytes = new byte[]
        {
                0x3B, 0x34, 0xA8, 0x4E,
                0xF2, 0x0A, 0xD7, 0xFE
        };

        var bdw = new BaseDataWindow(bytes, 8);

        Assert.Equal(result, bdw.GetUInt32(index, offset, bitCount));
    }

    [Theory]
    [InlineData(0, 8, 2, 0x82, new byte[] { 0x0B, 0x36, 0xA8, 0x4E, 0xF2, 0x0A, 0xD7, 0xFE })]
    public void SetUInt32SetsCorrectValue(int index, int bitCount, int offset, uint value, byte[] resultingArray)
    {
        byte[] bytes = new byte[]
        {
                0x3B, 0x34, 0xA8, 0x4E,
                0xF2, 0x0A, 0xD7, 0xFE
        };

        var bdw = new BaseDataWindow(bytes, 8);

        bdw.SetUInt32(index, offset, bitCount, value);

        Assert.Equal(resultingArray, bdw.Data);
    }

    [Theory]
    [InlineData(new byte[] { 0x0B, 0x36, 0xA8, 0x4E, 0xF2, 0x0A, 0xD7, 0xFE }, nameof(InheritorOfBaseDataWindow) + "(CzaoTvIK1/4=)")]
    [InlineData(new byte[] { 0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49, 0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69,
                                 0xFE, 0x03, 0x18, 0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
                                }, nameof(InheritorOfBaseDataWindow) + "(R2FsbGFkZQAAAAAt9pwBSf/kUgrKTPkPG8SsUGn+Axh4xet2AAAAAAAAAABAAAAA)")]
    public void SerializationWorks(byte[] input, string expected)
    {
        var bdc = new InheritorOfBaseDataWindow(input, input.Length);

        Assert.Equal(expected, bdc.Serialize());
    }

    class InheritorOfBaseDataWindow : BaseDataWindow
    {
        public InheritorOfBaseDataWindow(byte[] data, int length, bool doCompressionWhenSerializing = false) : base(data, length, doCompressionWhenSerializing) { }
    }

    [Theory]
    [InlineData(new byte[] { 0x0B, 0x36, 0xA8, 0x4E, 0xF2, 0x0A, 0xD7, 0xFE }, nameof(InheritorOfBaseDataWindow) + "(CzaoTvIK1/4=)")]
    [InlineData(new byte[] { 0x47, 0x61, 0x6C, 0x6C, 0x61, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49, 0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0xAC, 0x50, 0x69,
                                 0xFE, 0x03, 0x18, 0x78, 0xC5, 0xEB, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
                                }, nameof(InheritorOfBaseDataWindow) + "(R2FsbGFkZQAAAAAt9pwBSf/kUgrKTPkPG8SsUGn+Axh4xet2AAAAAAAAAABAAAAA)")]
    public void DeserializationWorks(byte[] expected, string input)
    {
        var bdc = new InheritorOfBaseDataWindow(new byte[expected.Length], expected.Length);

        Assert.True(bdc.TryDeserialize(input));

        Assert.Equal(expected, bdc.Data);
    }


}
