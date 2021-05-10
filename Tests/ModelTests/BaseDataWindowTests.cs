using Core.Models;
using System;
using Xunit;

namespace Tests.ModelTests
{
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

            Assert.Equal(result, bdw.GetUInt32(index, bitCount, offset));
        }

        [Theory]
        [InlineData(0, 8, 2, 0x82, new byte[] { 0x0B, 0x36, 0xA8, 0x4E, 0xF2, 0x0A, 0xD7, 0xFE})]
        public void SetUInt32SetsCorrectValue(int index, int bitCount, int offset, uint value, byte[] resultingArray)
        {
            byte[] bytes = new byte[]
            {
                0x3B, 0x34, 0xA8, 0x4E,
                0xF2, 0x0A, 0xD7, 0xFE
            };

            var bdw = new BaseDataWindow(bytes, 8);

            bdw.SetUInt32(index, bitCount, offset, value);

            Assert.Equal(resultingArray, bdw.Data);
        }
    }
}
