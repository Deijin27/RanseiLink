using RanseiLink.Core.Graphics;
using System.IO;

namespace RanseiLink.CoreTests.GraphicsTests;

public class NsbtxTests
{
    [Fact]
    public void TestMap_LoadCorrectly()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_map2.nsbtx");
        File.Exists(file).Should().BeTrue();

        var btx = new NSBTX(file);
        var tex = btx.Texture;
        tex.Should().NotBeNull();

        tex.Textures.Should().NotBeNull().And.HaveCount(12);
        tex.Textures[0].Name.Should().Be("map02_00_01");
        tex.Textures[1].Name.Should().Be("map02_00_02a");
        tex.Textures[2].Name.Should().Be("map02_00_03");
        tex.Textures[3].Name.Should().Be("map02_00_04");
        tex.Textures[4].Name.Should().Be("map02_00_06a");
        tex.Textures[5].Name.Should().Be("map02_00_07");
        tex.Textures[6].Name.Should().Be("map02_00_08a");
        tex.Textures[7].Name.Should().Be("map02_00_09a");
        tex.Textures[8].Name.Should().Be("map02_00_10");
        tex.Textures[9].Name.Should().Be("map02_00_10a");
        tex.Textures[10].Name.Should().Be("map02_00_11");
        tex.Textures[11].Name.Should().Be("map02_00_12");

        tex.Textures[0].TextureData.Should().StartWith(new byte[] { 0x33, 0x3B, 0x4C, 0x5E, 0x48, 0x41, 0x4D, 0x63 });

        //tex.Textures[0].TextureData.Should().HaveCount(0x100);
        //tex.Textures[1].TextureData.Should().HaveCount(0x20);
        //tex.Textures[2].TextureData.Should().HaveCount(0x100);
        //tex.Textures[3].TextureData.Should().HaveCount(0x100);
        //tex.Textures[4].TextureData.Should().HaveCount(0x20);
        //tex.Textures[5].TextureData.Should().HaveCount(0x100);
        //tex.Textures[6].TextureData.Should().HaveCount(0x100);
        //tex.Textures[7].TextureData.Should().HaveCount(0x20);
        //tex.Textures[8].TextureData.Should().HaveCount(0x100);
        //tex.Textures[9].TextureData.Should().HaveCount(0x20);
        //tex.Textures[10].TextureData.Should().HaveCount(0x100);
        //tex.Textures[11].TextureData.Should().HaveCount(0x100);

        tex.Palettes.Should().NotBeNull().And.HaveCount(12);
        tex.Palettes[0].Name.Should().Be("map02_00_01_pl");
        tex.Palettes[1].Name.Should().Be("map02_00_02a_pl");
        tex.Palettes[2].Name.Should().Be("map02_00_03_pl");
        tex.Palettes[3].Name.Should().Be("map02_00_04_pl");
        tex.Palettes[4].Name.Should().Be("map02_00_06a_pl");
        tex.Palettes[5].Name.Should().Be("map02_00_07_pl");
        tex.Palettes[6].Name.Should().Be("map02_00_08a_pl");
        tex.Palettes[7].Name.Should().Be("map02_00_09a_pl");
        tex.Palettes[8].Name.Should().Be("map02_00_10_pl");
        tex.Palettes[9].Name.Should().Be("map02_00_10a_pl");
        tex.Palettes[10].Name.Should().Be("map02_00_11_pl");
        tex.Palettes[11].Name.Should().Be("map02_00_12_pl");

        tex.Palettes[0].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[1].PaletteData.Should().HaveCount(0x20);
        tex.Palettes[2].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[3].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[4].PaletteData.Should().HaveCount(0x20);
        tex.Palettes[5].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[6].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[7].PaletteData.Should().HaveCount(0x20);
        tex.Palettes[8].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[9].PaletteData.Should().HaveCount(0x20);
        tex.Palettes[10].PaletteData.Should().HaveCount(0x100);
        tex.Palettes[11].PaletteData.Should().HaveCount(0x100);
    }

    [Fact]
    public void TestMap_IdenticalThroughLoadSaveCycle()
    {
        string fileName = "test_map2.nsbtx";
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var btx = new NSBTX(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        btx.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
