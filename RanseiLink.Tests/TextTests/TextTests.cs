﻿using RanseiLink.Core.Text;
using Xunit;

namespace RanseiLink.Tests.TextTests
{
    public class TextTests
    {
        [Theory]
        [InlineData(new byte[] { 0x42, 0x6F, 0x6B, 0x75, 0x7A, 0x65, 0x6E }, "Bokuzen")]
        [InlineData(new byte[] { 0x4D, 0x61, 0x61 }, "Maa")]
        [InlineData(new byte[] { 0x47, 0x65, 0x6E, 0x27, 0x61, 0x6E }, "Gen'an")]
        [InlineData(new byte[] { 0x43, 0x68, 0x84, 0x90, 0x61, 0x6E }, "Chōan")]
        [InlineData(new byte[] { 0x4A, 0x84, 0x81, 0x62, 0x65, 0x69 }, "Jūbei")]
        [InlineData(new byte[] { 0x83, 0x5E, 0x83, 0x50, 0x83, 0x56 }, "タケシ")]
        [InlineData(new byte[] { 0x82, 0xE2, 0x82, 0xDC, 0x82, 0xA8, 0x82, 0xC6, 0x82, 0xB1 }, "やまおとこ")]
        [InlineData(new byte[] { 0x83, 0x47, 0x83, 0x8A, 0x81, 0x5B, 0x83, 0x67, 0x83, 0x67, 0x83, 0x8C, 0x81, 0x5B, 0x83, 0x69, 0x81, 0x5B }, "エリートトレーナー")]
        public void ReadConvertsNameCorrectly(byte[] input, string expected)
        {
            var reader = new PNAReader(input, false, true);
            Assert.Equal(expected, reader.Text);
        }

        [Theory]
        [InlineData(new byte[] { 0x42, 0x6F, 0x6B, 0x75, 0x7A, 0x65, 0x6E }, "Bokuzen")]
        [InlineData(new byte[] { 0x4D, 0x61, 0x61 }, "Maa")]
        [InlineData(new byte[] { 0x47, 0x65, 0x6E, 0x27, 0x61, 0x6E }, "Gen'an")]
        [InlineData(new byte[] { 0x43, 0x68, 0x84, 0x90, 0x61, 0x6E }, "Chōan")]
        [InlineData(new byte[] { 0x4A, 0x84, 0x81, 0x62, 0x65, 0x69 }, "Jūbei")]
        [InlineData(new byte[] { 0x83, 0x5E, 0x83, 0x50, 0x83, 0x56 }, "タケシ")]
        [InlineData(new byte[] { 0x82, 0xE2, 0x82, 0xDC, 0x82, 0xA8, 0x82, 0xC6, 0x82, 0xB1 }, "やまおとこ")]
        [InlineData(new byte[] { 0x83, 0x47, 0x83, 0x8A, 0x81, 0x5B, 0x83, 0x67, 0x83, 0x67, 0x83, 0x8C, 0x81, 0x5B, 0x83, 0x69, 0x81, 0x5B }, "エリートトレーナー")]
        public void WriteConvertsNameCorrectly(byte[] expected, string input)
        {
            var reader = new PNAWriter(input, false, true);
            Assert.Equal(expected, reader.Data);
        }
    }
}