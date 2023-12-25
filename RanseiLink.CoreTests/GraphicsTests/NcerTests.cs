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

        ncer.CellBanks.Ucat.Should().NotBeNull();

        ncer.Labels.Names.Should().ContainSingle()
            .Which.Should().Be("CellAnime0");

        ncer.Unknown.Unknown.Should().Be(0);
    }

    [Fact]
    public void LoadKiAuroraAnim()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_ki2_aurora_anim.ncer"));

        // Cell section
        ncer.CellBanks.BlockSize.Should().Be(2);
        ncer.CellBanks.BankType.Should().Be(1);
        var banks = ncer.CellBanks.Banks;
        banks.Should().HaveCount(12);

        var bank0 = banks[0];
        using (new AssertionScope())
        {
            bank0.ReadOnlyCellInfo.Should().Be(0x800);
            bank0.XMax.Should().Be(0);
            bank0.XMin.Should().Be(0);
            bank0.YMax.Should().Be(0);
            bank0.YMin.Should().Be(0);
            bank0.Should().BeEmpty();
        }

        var bank1 = banks[1];
        using (new AssertionScope())
        {
            bank1.ReadOnlyCellInfo.Should().Be(0x829);
            bank1.XMax.Should().Be(160);
            bank1.XMin.Should().Be(97);
            bank1.YMax.Should().Be(34);
            bank1.YMin.Should().Be(3);
            bank1.Should().HaveCount(1);
        }
        var cell = bank1[0];
        using (new AssertionScope())
        {
            cell.Height.Should().Be(32);
            cell.Width.Should().Be(64);
            cell.XOffset.Should().Be(97);
            cell.YOffset.Should().Be(3);
            cell.TileOffset.Should().Be(0);

            cell.Depth.Should().Be(BitDepth.e8Bit);
            cell.FlipX.Should().BeFalse();
            cell.FlipY.Should().BeFalse();
            cell.DoubleSize.Should().BeFalse();
            cell.Mosaic.Should().BeFalse();
            cell.IndexPalette.Should().Be(0);
            cell.RotateOrScale.Should().Be(RotateOrScale.Rotate);
            cell.Priority.Should().Be(0);
            cell.SelectParam.Should().Be(0);
            cell.Shape.Should().Be(Shape.Wide);
            cell.Scale.Should().Be(Scale.XLarge);
        }

        ncer.CellBanks.Ucat.Should().BeNull();

        // Labels section
        ncer.Labels.Names.Should().Equal(new string[] { "reflection", "flag_r", "flag_l" });

        // Unknown section
        ncer.Unknown.Unknown.Should().Be(0);
    }

    [Theory]
    [InlineData("test_minimap_9.ncer")]
    [InlineData("test_warriorbattleintro.ncer")]
    [InlineData("test_ki2_aurora_anim.ncer")]
    [InlineData("test_cma_ignis.ncer")]
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

        //File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
