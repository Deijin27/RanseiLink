using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

/// <summary>
/// This verifies that conversions of ncer-sprites with 1 cell bank are working
/// </summary>
public class StlTests
{
    private static CellImageSettings Settings => STL.Settings with { ScaleDimensionsToFitCells = true };

    [Theory]
    [InlineData("test_warriorbattleintro")]
    [InlineData("test_pokemonxl")]
    public void ConvertToImage(string filename)
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".nclr"));
        var png = Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".png");

        ncer.CellBanks.Banks.Should().HaveCount(1);

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Settings);

        using var expectedImage = Image.Load<Rgba32>(png);
        image.ShouldBeIdenticalTo(expectedImage);
    }

    [Theory]
    [InlineData("test_warriorbattleintro")]
    [InlineData("test_pokemonxl")]
    public void IdenticalThroughConversionCycle(string filename)
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, filename + ".nclr"));

        var oldPixels = ncgr.Pixels.Data.ToArray();
        var oldPalette = PaletteUtil.To32bitColors(nclr.Palettes.Palette);

        using var image = NitroImageUtil.NcerToImage(ncer, ncgr, nclr, Settings);

        var info = CellImageUtil.MultiBankFromImage(
            image,
            ncer.CellBanks.Banks,
            ncer.CellBanks.BlockSize,
            ncgr.Pixels.IsTiled,
            ncgr.Pixels.Format,
            STL.Settings
            );

        var newPixels = info.Pixels;
        var newPalette = info.Palette.Should().ContainSingle().Which;

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