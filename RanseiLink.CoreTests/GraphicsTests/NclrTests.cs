using RanseiLink.Core.Graphics;
using System.IO;

namespace RanseiLink.CoreTests.GraphicsTests;

public class NclrTests
{
    [Fact]
    public void LoadNclrSingle()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_single.nclr");
        File.Exists(file).Should().BeTrue();

        var ncgr = NCLR.Load(file);

        ncgr.PaletteCollectionMap.Should().BeNull();
        var plt = ncgr.Palettes;
        plt.Should().NotBeNull();

        plt.Format.Should().Be(TexFormat.Pltt256);
        plt.Palette.Should().HaveCount(0x100);
        plt.Palette[^1].Should().Be(new Rgb15(29, 29, 29));
        plt.Palette[^2].Should().Be(new Rgb15(28, 28, 28));
    }

    [Fact]
    public void LoadNclrMulti()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_multi.nclr");
        File.Exists(file).Should().BeTrue();

        var ncgr = NCLR.Load(file);

        
        var plt = ncgr.Palettes;
        plt.Should().NotBeNull();
        plt.Format.Should().Be(TexFormat.Pltt256);
        plt.Palette.Should().HaveCount(0x100);
        plt.Palette[0].Should().Be(new Rgb15(16, 31, 0));

        var pcm = ncgr.PaletteCollectionMap;
        pcm.Should().NotBeNull();
        pcm!.Palettes.Should().Equal(new ushort[] { 0x00 });
    }

    [Theory]
    [InlineData("test_single.nclr")]
    [InlineData("test_multi.nclr")]
    [InlineData("test_cma_ignis.nclr")]
    [InlineData("test_kico_ignis.nclr")]
    [InlineData("test_ki2_aurora_anim.nclr")]
    public void LoadSaveCycle(string fileName)
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var ncgr = NCLR.Load(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        ncgr.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
