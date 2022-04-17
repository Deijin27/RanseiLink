using RanseiLink.Core.Models;
using System.IO;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace RanseiLink.CoreTests.ModelTests;

/// <summary>
/// At the moment manually add the event/00065.eve and 64.eve to the test mod folder for these tests to work.
/// Since they're currently not part of mods.
/// </summary>
public class EventTests
{
    private static string TestTempFolder => Path.Combine(TestConstants.TestTempFolder, "Event");

    public EventTests()
    {
        Directory.CreateDirectory(TestTempFolder);
    }

    [Theory]
    [InlineData("00000064.eve", 0x64, 0x31C8, 0x1B, 0x1A2)]
    [InlineData("00000065.eve", 0x65, 0x2C10, 0x1B, 0x86)]
    public void ReadsFileCorrectly(string eventFileName, int fileId, int headerUnknown, int eventGroupCount, int subgroupCount)
    {
        string eventFile = Path.Combine(TestConstants.TestModFolder, "event", eventFileName);

        var eve = new EVE(eventFile);

        eve.FileId.Should().Be(fileId);
        eve.HeaderUnknown.Should().Be(headerUnknown);

        eve.EventGroupsA.Should().HaveCount(eventGroupCount);
        eve.EventGroupsB.Should().HaveCount(eventGroupCount);

        eve.EventGroupsA.Sum(i => i.Count).Should().Be(subgroupCount);
        eve.EventGroupsB.Sum(i => i.Count).Should().Be(subgroupCount);
    }

    [Theory]
    [InlineData("00000064.eve")]
    [InlineData("00000065.eve")]
    public void IdenticalThroughLoadSaveCycle(string eventFileName)
    {
        string eventFile = Path.Combine(TestConstants.TestModFolder, "event", eventFileName);

        var eve = new EVE(eventFile);

        string tempFile = Path.Combine(TestTempFolder, "saved.eve");

        eve.Save(tempFile);
        
        var expectedBytes = File.ReadAllBytes(eventFile);
        var actualBytes = File.ReadAllBytes(tempFile);

        actualBytes.Should().Equal(expectedBytes);
    }

    [Theory]
    [InlineData("00000064.eve")]
    [InlineData("00000065.eve")]
    public void IndenticalThroughUnpackPack(string eventFileName)
    {
        string eventFile = Path.Combine(TestConstants.TestModFolder, "event", eventFileName);

        string tempDir = Path.Combine(TestTempFolder, "unpacked");
        if (Directory.Exists(tempDir)) 
        { 
            Directory.Delete(tempDir, true); 
        }
        Directory.CreateDirectory(tempDir);
        EVE.Unpack(eventFile, tempDir);
        string tempFile = Path.Combine(TestTempFolder, "packed.eve");
        EVE.Pack(tempDir, tempFile);

        var expectedBytes = File.ReadAllBytes(eventFile);
        var actualBytes = File.ReadAllBytes(tempFile);

        actualBytes.Should().Equal(expectedBytes);
    }
}