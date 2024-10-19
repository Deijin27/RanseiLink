using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

public class CellAnimSerialiseTests
{
    [Fact]
    public void ExportFormat_LoadSaveCycle()
    {
        // Validate that the format we export animations as remains identical through load and save

        var doc = XDocument.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_ki2_aurora_anim_serialised.xml"));

        var res = new RLAnimationResource(doc);
        var newDoc = res.Serialise();

        newDoc.Should().BeEquivalentTo(doc);
    }

    [Theory]
    [InlineData("test_ki2_aurora_anim", RLAnimationFormat.OneImagePerCluster, AnimationTypeId.KuniImage2)]
    [InlineData("test_ki2_aurora_anim", RLAnimationFormat.OneImagePerCell, AnimationTypeId.KuniImage2)]
    //[InlineData("test_cma_ignis", RLAnimationFormat.OneImagePerCluster, AnimationTypeId.Castlemap)] // this one differs because part of one of the cells gets cropped
    [InlineData("test_cma_ignis", RLAnimationFormat.OneImagePerCell, AnimationTypeId.Castlemap)]
    [InlineData("test_kico_ignis", RLAnimationFormat.OneImagePerCell, AnimationTypeId.IconCastle, false)]
    public void Convert(string testFileName, RLAnimationFormat fmt, AnimationTypeId type, bool checkClusterMinMax = true)
    {
        var prt = AnimationTypeInfoResource.Get(type).Prt;
        string ncerPath = Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.ncer");
        var nanr = NANR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nanr"));
        var ncer = NCER.Load(ncerPath);
        var newNcer = NCER.Load(ncerPath);
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.nclr"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, $"{testFileName}.ncgr"));

        var oldPalette = new PaletteCollection(nclr.Palettes.Palette, ncgr.Pixels.Format, true);
        var oldPixels = ncgr.Pixels.Data.ToArray();

        int width = 256;
        int height = 120;
        var settings = new CellImageSettings(prt);
        string? backgroundFileName = null;
        var exportData = CellAnimationSerialiser.ExportAnimationXml(nanr, ncer, ncgr, nclr, width, height, settings, fmt, backgroundFileName);

        var newNanr = CellAnimationSerialiser.ImportAnimationXml(exportData, newNcer, ncgr, nclr, width, height, settings);

        var newPalette = new PaletteCollection(nclr.Palettes.Palette, ncgr.Pixels.Format, true);
        var newPixels = ncgr.Pixels.Data;

        
        GraphicsAssertions.AssertNanrEqual(nanr, newNanr);
        GraphicsAssertions.AssertNcerEqual(ncer, newNcer, checkClusterMinMax);
        // We can't do exact palette equivalency tests because there are some
        // unused colors in the original palettes which get lost in conversion
        GraphicsAssertions.CellPixelsAreEquivalent(ncer.Clusters, newNcer.Clusters, ncgr.Pixels.Format, oldPixels, newPixels, oldPalette, newPalette);
    }
}
