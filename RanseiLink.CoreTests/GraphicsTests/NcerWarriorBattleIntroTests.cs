using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

/// <summary>
/// This verifies that conversions of ncer-sprites with 1 cell bank are working
/// </summary>
public class NcerWarriorBattleIntroTests
{
    [Fact]
    public void ConvertToImage()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.nclr"));
        var png = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.png");

        var oldPixels = ncgr.Pixels.Data;
        var oldPalette = RawPalette.To32bitColors(nclr.Palettes.Palette);

        ncer.CellBanks.Banks.Should().HaveCount(1);

        using var image = CellImageUtil.MultiBankToImage(
            ncer.CellBanks.Banks,
            ncer.CellBanks.BlockSize,
            new SpriteImageInfo(oldPixels, oldPalette, -1, -1),
            ncgr.Pixels.IsTiled,
            debug: false,
            format: ncgr.Pixels.Format
            );

        using var expectedImage = Image.Load<Rgba32>(png);
        image.ShouldBeIdenticalTo(expectedImage);
    }

    [Fact]
    public void IdenticalThroughConversionCycle()
    {
        var ncer = NCER.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.ncer"));
        var ncgr = NCGR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.ncgr"));
        var nclr = NCLR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_warriorbattleintro.nclr"));

        var oldPixels = ncgr.Pixels.Data;
        var oldPalette = RawPalette.To32bitColors(nclr.Palettes.Palette);

        using var image = CellImageUtil.MultiBankToImage(
            ncer.CellBanks.Banks,
            ncer.CellBanks.BlockSize,
            new SpriteImageInfo(oldPixels, oldPalette, -1, -1),
            ncgr.Pixels.IsTiled,
            debug: false,
            format: ncgr.Pixels.Format
            );

        var info = CellImageUtil.MultiBankFromImage(
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
