using FluentAssertions;
using RanseiLink.Core.Graphics;
using System.IO;
using Xunit;

namespace RanseiLink.CoreTests.GraphicsTests;

public class NsbmdTests
{

    [Fact]
    public void TestMap_LoadCorrectly()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_map3.nsbmd");
        File.Exists(file).Should().BeTrue();

        var bmd = new NSBMD(file);
        var mdl = bmd.Model;
        mdl.Should().NotBeNull();

        mdl.Models.Should().NotBeNull().And.HaveCount(1);

        var mod = mdl.Models[0];

        mod.Name.Should().Be("MAP03_00");

        var mdlInfo = mod.MdlInfo;

        var polymeshes = mod.Polymeshes;
        polymeshes.Should().NotBeNull().And.HaveCount(28);
        polymeshes[0].Name.Should().Be("set-map03_00");
        polymeshes[1].Name.Should().Be("map03_00");
        polymeshes[2].Name.Should().Be("polymsh");
        polymeshes[3].Name.Should().Be("polymsh1");
        polymeshes[4].Name.Should().Be("polymsh14");
        polymeshes[5].Name.Should().Be("polymsh16");
        polymeshes[6].Name.Should().Be("polymsh19");
        polymeshes[7].Name.Should().Be("polymsh2");
        polymeshes[8].Name.Should().Be("polymsh20");
        polymeshes[9].Name.Should().Be("polymsh24");
        polymeshes[10].Name.Should().Be("polymsh3");
        polymeshes[11].Name.Should().Be("polymsh888");
        polymeshes[12].Name.Should().Be("polymsh889");
        polymeshes[13].Name.Should().Be("polymsh890");
        polymeshes[14].Name.Should().Be("polymsh891");
        polymeshes[15].Name.Should().Be("polymsh892");
        polymeshes[16].Name.Should().Be("polymsh896");
        polymeshes[17].Name.Should().Be("polymsh897");
        polymeshes[18].Name.Should().Be("polymsh898");
        polymeshes[19].Name.Should().Be("map03_00am");
        polymeshes[20].Name.Should().Be("polymsh_m03");
        polymeshes[21].Name.Should().Be("polymsh_m04");
        polymeshes[22].Name.Should().Be("polymsh_m05");
        polymeshes[23].Name.Should().Be("polymsh_m12");
        polymeshes[24].Name.Should().Be("polymsh_m13");
        polymeshes[25].Name.Should().Be("polymsh_m14");
        polymeshes[26].Name.Should().Be("polymsh_m7");
        polymeshes[27].Name.Should().Be("polymsh_m9");

        var renderCommands = mod.RenderCommands;
        renderCommands.Should().NotBeNull();

        var materials = mod.Materials;
        materials.Should().NotBeNull().And.HaveCount(12);

        var material0 = materials[0];
        material0.Name.Should().Be("map03_00_01");
        material0.Texture.Should().Be("map03_00_01");
        material0.Palette.Should().Be("map03_00_01_pl");

        var material1 = materials[1];
        material1.Name.Should().Be("map03_00_01a");
        material1.Texture.Should().Be("map03_00_01a");
        material1.Palette.Should().Be("map03_00_01a_pl");

        var material2 = materials[2];
        material2.Name.Should().Be("map03_00_02");
        material2.Texture.Should().Be("map03_00_02");
        material2.Palette.Should().Be("map03_00_02_pl");

        var material3 = materials[3];
        material3.Name.Should().Be("map03_00_03");
        material3.Texture.Should().Be("map03_00_03");
        material3.Palette.Should().Be("map03_00_03_pl");

        var material4 = materials[4];
        material4.Name.Should().Be("map03_00_03a");
        material4.Texture.Should().Be("map03_00_03a");
        material4.Palette.Should().Be("map03_00_03a_pl");

        var material5 = materials[5];
        material5.Name.Should().Be("map03_00_04");
        material5.Texture.Should().Be("map03_00_04");
        material5.Palette.Should().Be("map03_00_04_pl");

        var material6 = materials[6];
        material6.Name.Should().Be("map03_00_05");
        material6.Texture.Should().Be("map03_00_05");
        material6.Palette.Should().Be("map03_00_05_pl");

        var material7 = materials[7];
        material7.Name.Should().Be("map03_00_06");
        material7.Texture.Should().Be("map03_00_06");
        material7.Palette.Should().Be("map03_00_06_pl");

        var material8 = materials[8];
        material8.Name.Should().Be("map03_00_07");
        material8.Texture.Should().Be("map03_00_07");
        material8.Palette.Should().Be("map03_00_07_pl");

        var material9 = materials[9];
        material9.Name.Should().Be("map03_00_08");
        material9.Texture.Should().Be("map03_00_08");
        material9.Palette.Should().Be("map03_00_08_pl");

        var material10 = materials[10];
        material10.Name.Should().Be("map03_00_11a");
        material10.Texture.Should().Be("map03_00_11a");
        material10.Palette.Should().Be("map03_00_11a_pl");

        var material11 = materials[11];
        material11.Name.Should().Be("map03_00_13a_m00");
        material11.Texture.Should().Be("map03_00_13a");
        material11.Palette.Should().Be("map03_00_13a_pl");

        var polygons = mod.Polygons;
        polygons.Should().NotBeNull().And.HaveCount(25);
        polygons[0].Name.Should().Be("polygon0");
        polygons[1].Name.Should().Be("polygon1");
        polygons[2].Name.Should().Be("polygon2");
        polygons[3].Name.Should().Be("polygon3");
        polygons[4].Name.Should().Be("polygon4");
        polygons[5].Name.Should().Be("polygon5");
        polygons[6].Name.Should().Be("polygon6");
        polygons[7].Name.Should().Be("polygon7");
        polygons[8].Name.Should().Be("polygon8");
        polygons[9].Name.Should().Be("polygon9");
        polygons[10].Name.Should().Be("polygon10");
        polygons[11].Name.Should().Be("polygon11");
        polygons[12].Name.Should().Be("polygon12");
        polygons[13].Name.Should().Be("polygon13");
        polygons[14].Name.Should().Be("polygon14");
        polygons[15].Name.Should().Be("polygon15");
        polygons[16].Name.Should().Be("polygon16");
        polygons[17].Name.Should().Be("polygon17");
        polygons[18].Name.Should().Be("polygon18");
        polygons[19].Name.Should().Be("polygon19");
        polygons[20].Name.Should().Be("polygon20");
        polygons[21].Name.Should().Be("polygon21");
        polygons[22].Name.Should().Be("polygon22");
        polygons[23].Name.Should().Be("polygon23");
        polygons[24].Name.Should().Be("polygon24");

    }

    [Theory]
    [InlineData("test_map0.nsbmd")]
    [InlineData("test_map3.nsbmd")]
    [InlineData("test_map5.nsbmd")]
    public void TestMap_IdenticalThroughLoadSaveCycle(string fileName)
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var bmd = new NSBMD(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        bmd.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
