using RanseiLink.Core.Graphics;
using System.IO;

namespace RanseiLink.CoreTests.GraphicsTests;

/// <summary>
/// Basic tests for ncer loading
/// </summary>
public class NcerTests
{
    /// <summary>
    /// Notable, this has only one label but multiple banks
    /// </summary>
    [Fact]
    public void LoadMinimap()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncer"));

        ncer.CellBanks.BlockSize.Should().Be(2);
        var banks = ncer.CellBanks.Banks;
        banks.Should().HaveCount(4);

        ncer.Labels.Names.Should().ContainSingle()
            .Which.Should().Be("CellAnime0");

        ncer.Unknown.Unknown.Should().Be(0);
    }

    [Theory]
    [InlineData("test_minimap_9.ncer")]
    public void LoadSaveCycle(string fileName)
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var ncer = NCER.Load(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        ncer.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
