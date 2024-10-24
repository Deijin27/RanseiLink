using RanseiLink.Core.Archive;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.ArchiveTests;

public class LinkTests
{
    [Fact]
    public void UnpackContainsExpectedFiles()
    {
        var testFileName = "test_kico_ignis";
        var ncerPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.ncer");
        var nanrPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nanr");
        var nclrPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nclr");
        var ncgrPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.ncgr");

        var linkPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.link");

        var temp = Path.Combine(TestConstants.TestTempFolder, $"{nameof(UnpackContainsExpectedFiles)}_{testFileName}");
        if (Directory.Exists(temp))
        {
            Directory.Delete(temp, true);
        }

        LINK.Unpack(linkPath, temp);

        Directory.Exists(temp).Should().BeTrue();

        var files = Directory.GetFiles(temp);
        files.Length.Should().Be(6);

        var fileNames = files.Select(Path.GetFileName).OrderBy(x => x).ToArray();

        string[] expectedFileNames = ["0000.nanr", "0001.ncgr", "0002.ncer", "0003.ncgr", "0004.nclr", "0005.nscr"];

        fileNames.Should().Equal(expectedFileNames);

        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0000.nanr"), nanrPath);
        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0001.ncgr"), ncgrPath);
        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0002.ncer"), ncerPath);
        AssertUtil.FilesShouldBeIdentical(Path.Combine(temp, "0004.nclr"), nclrPath);
        AssertUtil.FileShouldBeEmpty(Path.Combine(temp, "0003.ncgr"));
        AssertUtil.FileShouldBeEmpty(Path.Combine(temp, "0005.nscr"));

        Directory.Delete(temp, true);
    }

    [Fact]
    public void IdenticalThroughUnpackPackCycle()
    {
        var testFileName = "test_kico_ignis";

        var linkPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.link");

        var temp = Path.Combine(TestConstants.TestTempFolder, $"{nameof(IdenticalThroughUnpackPackCycle)}_{testFileName}");
        if (Directory.Exists(temp))
        {
            Directory.Delete(temp, true);
        }

        LINK.Unpack(linkPath, temp);

        var tempRepacked = Path.Combine(TestConstants.TestTempFolder, $"{nameof(IdenticalThroughUnpackPackCycle)}_{testFileName}_repacked.link");
        if (File.Exists(tempRepacked))
        {
            File.Delete(tempRepacked);
        }

        LINK.Pack(temp, tempRepacked);

        AssertUtil.FilesShouldBeIdentical(tempRepacked, linkPath);

        Directory.Delete(temp, true);
        File.Delete(tempRepacked);

    }
}
