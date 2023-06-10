using RanseiLink.Core.Graphics;
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
    [Fact]
    public void ConvertToImage()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.nclr"));
        var png = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.png");

        var oldPixels = ncgr.Pixels.Data;
        var oldPalette = RawPalette.To32bitColors(nclr.Palettes.Palette);

        using var image = ImageUtil.ToImage(
            ncer.CellBanks.Banks,
            ncer.CellBanks.BlockSize,
            new SpriteImageInfo(oldPixels, oldPalette, -1, -1),
            ncgr.Pixels.IsTiled,
            debug: false,
            ncgr.Pixels.Format
            );

        using var expectedImage = Image.Load<Rgba32>(png);
        image.ShouldBeIdenticalTo(expectedImage);
    }

    [Fact]
    public void IdenticalThroughConversionCycle()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_minimap_9.nclr"));

        var oldPixels = ncgr.Pixels.Data;
        var oldPalette = RawPalette.To32bitColors(nclr.Palettes.Palette);

        using var image = ImageUtil.ToImage(
            ncer.CellBanks.Banks,
            ncer.CellBanks.BlockSize,
            new SpriteImageInfo(oldPixels, oldPalette, -1, -1),
            ncgr.Pixels.IsTiled,
            debug: false,
            ncgr.Pixels.Format
            );

        var info = ImageUtil.FromImage(
            image, 
            ncer.CellBanks.Banks, 
            ncer.CellBanks.BlockSize, 
            ncgr.Pixels.IsTiled, 
            ncgr.Pixels.Format
            );

        var newPixels = info.Pixels;
        var newPalette = info.Palette;

        // Ensure palette has been maintained
        var oldPaletteSorted = oldPalette.Skip(1).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        var newPaletteSorted = newPalette.Skip(1).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        newPaletteSorted.Should().Equal(oldPaletteSorted);

        // Ensure pixels point to the same colors
        newPixels.Should().HaveSameCount(oldPixels);

        var oldPixelsAsColors = oldPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : oldPalette[x]).ToArray();
        var newPixelsAsColors = newPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : newPalette[x]).ToArray();

        newPixelsAsColors.Should().Equal(oldPixelsAsColors);
    }
}
