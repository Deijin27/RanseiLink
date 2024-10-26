using RanseiLink.Core.Archive;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.ArchiveTests;

public class PacTests
{
    [Fact]
    public void UnpackContainsExpectedFiles()
    {
        var testFileName = "test_map2";
        var nsbmdPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nsbmd");
        var nsbtaPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nsbta");
        var nsbtxPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nsbtx");

        var pacPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.pac");

        var temp = Path.Combine(TestConstants.TestTempFolder, $"{nameof(UnpackContainsExpectedFiles)}_{testFileName}");
        if (Directory.Exists(temp))
        {
            Directory.Delete(temp, true);
        }

        PAC.Unpack(pacPath, temp);

        Directory.Exists(temp).Should().BeTrue();

        var files = Directory.GetFiles(temp);
        files.Length.Should().Be(3);

        var fileNames = files.Select(Path.GetFileName).OrderBy(x => x).ToArray();

        string[] expectedFileNames = ["0000.nsbmd", "0001.nsbtx", "0002.nsbta"];

        fileNames.Should().Equal(expectedFileNames);

        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0000.nsbmd"), nsbmdPath);
        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0001.nsbtx"), nsbtxPath);
        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0002.nsbta"), nsbtaPath);

        Directory.Delete(temp, true);
    }

    [Fact]
    public void IdenticalThroughUnpackPackCycle()
    {
        var testFileName = "test_map2";

        var linkPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.pac");

        var temp = Path.Combine(TestConstants.TestTempFolder, $"{nameof(IdenticalThroughUnpackPackCycle)}_{testFileName}");
        if (Directory.Exists(temp))
        {
            Directory.Delete(temp, true);
        }

        PAC.Unpack(linkPath, temp);

        var tempRepacked = Path.Combine(TestConstants.TestTempFolder, $"{nameof(IdenticalThroughUnpackPackCycle)}_{testFileName}_repacked.pac");
        if (File.Exists(tempRepacked))
        {
            File.Delete(tempRepacked);
        }

        PAC.Pack(temp, tempRepacked, sharedFileCount: 0);

        AssertUtil.FilesShouldBeIdentical(tempRepacked, linkPath);

        Directory.Delete(temp, true);
        File.Delete(tempRepacked);
    }
}
