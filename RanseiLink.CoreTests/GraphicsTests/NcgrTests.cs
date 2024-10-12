using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

public class NcgrTests
{
    [Fact]
    public void LoadNcgrScreen()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_screen.ncgr");
        File.Exists(file).Should().BeTrue();

        var ncgr = NCGR.Load(file);

        var chr = ncgr.Pixels;
        chr.Should().NotBeNull();

        chr.Format.Should().Be(TexFormat.Pltt256);
        chr.IsTiled.Should().BeTrue();
        chr.TilesPerColumn.Should().Be(0x18);
        chr.TilesPerRow.Should().Be(0x20);
        chr.Unknown1.Should().Be(0);
        chr.Unknown2.Should().Be(0);
        chr.Data.Should().HaveCount(0xC000);
        chr.Data[..0x10].Should().Equal(new byte[] { 0xFB, 0xFA, 0xFC, 0xFB, 0xFC, 0xFA, 0xFC, 0xFA, 0xFB, 0xFC, 0xFA, 0xFC, 0xFA, 0xFC, 0xFA, 0xFC });
    }

    [Fact]
    public void LoadNcgrParts()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_parts.ncgr");
        File.Exists(file).Should().BeTrue();

        var ncgr = NCGR.Load(file);

        var chr = ncgr.Pixels;
        chr.Should().NotBeNull();

        chr.Format.Should().Be(TexFormat.Pltt256);
        chr.IsTiled.Should().BeFalse();
        chr.TilesPerColumn.Should().Be(-1);
        chr.TilesPerRow.Should().Be(-1);
        chr.Unknown1.Should().Be(0x10);
        chr.Unknown2.Should().Be(0x20);
        chr.Data.Should().HaveCount(0xA900);
        chr.Data.Skip(0x1870).Take(0x10).ToArray().Should().Equal(new byte[] { 0xB1, 0xB4, 0xBB, 0xD0, 0xC8, 0xC8, 0xE9, 0xF0, 0xF9, 0xF2, 0xF0, 0xF1, 0xEF, 0xEF, 0xEA, 0xEF });
    }

    [Theory]
    [InlineData("test_screen.ncgr")]
    [InlineData("test_parts.ncgr")]
    [InlineData("test_cma_ignis.ncgr")]
    [InlineData("test_kico_aurora.ncgr")]
    [InlineData("test_kico_ignis.ncgr")]
    [InlineData("test_ki2_aurora_anim.ncgr")]
    public void LoadSaveCycle(string fileName)
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var ncgr = NCGR.Load(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        ncgr.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
