﻿using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

/// <summary>
/// This verifies that conversions of ncer-sprites with multiple cell banks but one palette are working
/// </summary>
public class NcerMinimapTests
{
    private static CellImageSettings Settings => Core.Resources.NCERMiscItem.Settings;

    [Fact]
    public void ConvertToImage()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.nclr"));
        var png = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.png");

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Settings);

        using var expectedImage = Image.Load<Rgba32>(png);
        image.ShouldBeIdenticalTo(expectedImage);
    }

    [Fact]
    public void IdenticalThroughConversionCycle()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.nclr"));

        var oldPixels = ncgr.Pixels.Data.ToArray();
        var oldPalette = PaletteUtil.To32bitColors(nclr.Palettes.Palette);

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Settings);

        NitroImageUtil.NcerFromImage(ncer, ncgr, nclr, image, Settings);

        var newPixels = ncgr.Pixels.Data;
        var newPalette = PaletteUtil.To32bitColors(nclr.Palettes.Palette);
        // ensure only one palette, else we would need to check per cell instead
        newPalette.Should().HaveCount(ncgr.Pixels.Format.PaletteSize());

        GraphicsAssertions.PixelsAndPaletteAreEquivalent(oldPixels, newPixels, oldPalette, newPalette);
    }
}
