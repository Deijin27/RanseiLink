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
    private static CellImageSettings Settings => Core.Services.ModPatchBuilders.NcerPatchBuilder.Settings;

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

        // Ensure palette has been maintained
        Rgba32 black = Color.Black;
        newPalette.Should().HaveSameCount(oldPalette);
        // skip 1 because it's transparency which we don't care about
        // ignore black because it's used for padding
        var oldPaletteSorted = oldPalette.Skip(1).Where(x => x != black).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        var newPaletteSorted = newPalette.Skip(1).Where(x => x != black).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        newPaletteSorted.Should().Equal(oldPaletteSorted);

        // Ensure pixels point to the same colors
        newPixels.Should().HaveSameCount(oldPixels);

        var oldPixelsAsColors = oldPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : oldPalette[x]).ToArray();
        var newPixelsAsColors = newPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : newPalette[x]).ToArray();

        newPixelsAsColors.Should().Equal(oldPixelsAsColors);
    }
}
