﻿using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using System.IO;

namespace RanseiLink.CoreTests.TextTests;

public class TextTests
{
    private readonly IMsgService _msgService;
    public TextTests()
    {
        _msgService = new MsgService();
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
    public void ReadConvertsNameCorrectly(byte[] input, string expected)
    {
        _msgService.LoadName(input).Should().Be(expected);
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
        _msgService.SaveName(input).Should().Equal(expected);
    }

    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        var files = Directory.GetFiles(Path.Combine(TestConstants.TestModFolder, "msg"));
        files.Should().HaveCount(33);
        foreach (var file in files)
        {
            var unchangedBytes = File.ReadAllBytes(file);
            string temp = Path.GetTempFileName();
            var messages = _msgService.LoadBlock(file);
            _msgService.SaveBlock(temp, messages);
            var shouldBeUnchangedBytes = File.ReadAllBytes(temp);
            File.Delete(temp);

            shouldBeUnchangedBytes.Should().Equal(unchangedBytes);
            
        }
    }

    [Theory]
    [InlineData("VPYT")]
    [InlineData("VPYJ")]
    public void MsgDatIdenticalThroughUnpackPack(string gameCode)
    {
        string msgDat = Path.Combine(TestConstants.TestFolder, $"{gameCode}_MSG.DAT");
        IMsgService msgService = new MsgService();

        var unpackDir = Path.Combine(TestConstants.TestTempFolder, $"msg_unpack_{gameCode}");
        if (Directory.Exists(unpackDir))
        {
            Directory.Delete(unpackDir, true);
        }

        Directory.CreateDirectory(unpackDir);

        msgService.ExtractFromMsgDat(msgDat, unpackDir);

        var tempFile = Path.Combine(TestConstants.TestTempFolder, $"msg_new_{gameCode}");
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }

        msgService.CreateMsgDat(unpackDir, tempFile);

        File.ReadAllBytes(msgDat).Should().Equal(File.ReadAllBytes(tempFile));

        if (Directory.Exists(unpackDir))
        {
            Directory.Delete(unpackDir, true);
        }
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }
    }
}
