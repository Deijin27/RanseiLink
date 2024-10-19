using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

public static class GraphicsAssertions
{
    /// <summary>
    /// Assumes single palette which may not always be true for cell images
    /// </summary>
    public static void PixelsAndPaletteAreEquivalent(byte[] oldPixels, byte[] newPixels, Rgba32[] oldPalette, Rgba32[] newPalette)
    {
        PaletteIsEquivalent(oldPalette, newPalette);
        PixelsAreEquivalent(oldPixels, newPixels, oldPalette, newPalette);
    }

    private static void PaletteIsEquivalent(Rgba32[] oldPalette, Rgba32[] newPalette)
    {
        Rgba32 black = Color.Black;
        newPalette.Should().HaveSameCount(oldPalette);
        // skip 1 because it's transparency which we don't care about
        // ignore black because it's used for padding
        var oldPaletteSorted = oldPalette.Skip(1).Where(x => x != black).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        var newPaletteSorted = newPalette.Skip(1).Where(x => x != black).OrderBy(x => x.R).ThenBy(x => x.G).ThenBy(x => x.B).Distinct().ToArray();
        newPaletteSorted.Should().Equal(oldPaletteSorted);
    }

    private static void PixelsAreEquivalent(byte[] oldPixels, byte[] newPixels, Rgba32[] oldPalette, Rgba32[] newPalette)
    {
        // this only really works in the palette count is 1
        newPixels.Should().HaveSameCount(oldPixels);

        var oldPixelsAsColors = oldPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : oldPalette[x]).ToArray();
        var newPixelsAsColors = newPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : newPalette[x]).ToArray();

        newPixelsAsColors.Should().Equal(oldPixelsAsColors);
    }

    public static void CellPixelsAreEquivalent(CEBK oldCells, CEBK newCells, TexFormat format, byte[] oldPixels, byte[] newPixels, PaletteCollection oldPalette, PaletteCollection newPalette)
    {
        // the number of palettes should be the same, but unused colors may have been lost in conversion
        newPalette.Should().HaveSameCount(newPalette);
        // the number of pixels should be the same, but we must go cell by cell to be looking in the right palette
        // for the color which the pixel refers to.
        //newPixels.Should().HaveSameCount(oldPixels);
        for (int clusterId = 0; clusterId < oldCells.Clusters.Count; clusterId++)
        {
            var oldCluster = oldCells.Clusters[clusterId];
            var newCluster = newCells.Clusters[clusterId];

            for (int cellId = 0; cellId < oldCluster.Count; cellId++)
            {
                var oldCell = oldCluster[cellId];
                var newCell = newCluster[cellId];

                var oldCellPixels = CellImageUtil.GetCellPixels(oldCell, oldCells.BlockSize, format, oldPixels);
                var newCellPixels = CellImageUtil.GetCellPixels(newCell, newCells.BlockSize, format, newPixels);
                var oldPixelsAsColors = oldCellPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : oldPalette[oldCell.IndexPalette][x]).ToArray();
                var newPixelsAsColors = newCellPixels.Select(x => x == 0 ? (Rgba32)Color.Transparent : newPalette[newCell.IndexPalette][x]).ToArray();
                newPixelsAsColors.Should().Equal(oldPixelsAsColors);
            }
        }
    }

    public static void AssertNanrEqual(NANR oldNanr, NANR newNanr)
    {
        newNanr.Labels.Names.Should().Equal(oldNanr.Labels.Names);
        newNanr.Unknown.Unknown.Should().Be(oldNanr.Unknown.Unknown);

        newNanr.AnimationBanks.Banks.Should().HaveSameCount(oldNanr.AnimationBanks.Banks);

        for (int animId = 0; animId < newNanr.AnimationBanks.Banks.Count; animId++)
        {
            var newAnimBank = newNanr.AnimationBanks.Banks[animId];
            var oldAnimBank = oldNanr.AnimationBanks.Banks[animId];

            using (new AssertionScope())
            {
                newAnimBank.DataType.Should().Be(oldAnimBank.DataType);
                newAnimBank.Unknown1.Should().Be(oldAnimBank.Unknown1);
                newAnimBank.Unknown2.Should().Be(oldAnimBank.Unknown2);
                newAnimBank.Unknown3.Should().Be(oldAnimBank.Unknown3);
                newAnimBank.Frames.Should().HaveSameCount(oldAnimBank.Frames);
            }

            for (int frameId = 0; frameId < newAnimBank.Frames.Count; frameId++)
            {
                var newFrame = newAnimBank.Frames[frameId];
                var oldFrame = oldAnimBank.Frames[frameId];

                using (new AssertionScope())
                {
                    newFrame.Cluster.Should().Be(oldFrame.Cluster);
                    newFrame.Duration.Should().Be(oldFrame.Duration);
                }
            }
        }
    }

    public static void AssertNcerEqual(NCER oldNcer, NCER newNcer, bool checkClusterMinMax)
    {
        newNcer.Labels.Names.Should().Equal(oldNcer.Labels.Names);
        newNcer.Unknown.Unknown.Should().Be(oldNcer.Unknown.Unknown);

        newNcer.Clusters.Clusters.Should().HaveSameCount(oldNcer.Clusters.Clusters);

        for (int clusterId = 0; clusterId < newNcer.Clusters.Clusters.Count; clusterId++)
        {
            var newCluster = newNcer.Clusters.Clusters[clusterId];
            var oldCluster = oldNcer.Clusters.Clusters[clusterId];

            newCluster.Should().HaveSameCount(oldCluster);
            for (int cellId = 0; cellId < newCluster.Count; cellId++)
            {
                var newCell = newCluster[cellId];
                var oldCell = oldCluster[cellId];

                using (new AssertionScope())
                {
                    newCell.XOffset.Should().Be(oldCell.XOffset);
                    newCell.YOffset.Should().Be(oldCell.YOffset);
                    newCell.Shape.Should().Be(oldCell.Shape);
                    newCell.Scale.Should().Be(oldCell.Scale);
                    newCell.Width.Should().Be(oldCell.Width);
                    newCell.Height.Should().Be(oldCell.Height);
                    newCell.IndexPalette.Should().Be(oldCell.IndexPalette);
                    newCell.RotateOrScale.Should().Be(oldCell.RotateOrScale);
                    newCell.DoubleSize.Should().Be(oldCell.DoubleSize);
                    newCell.ObjMode.Should().Be(oldCell.ObjMode);
                    newCell.Mosaic.Should().Be(oldCell.Mosaic);
                    newCell.Depth.Should().Be(oldCell.Depth);
                    newCell.Unused.Should().Be(oldCell.Unused);
                    newCell.FlipX.Should().Be(oldCell.FlipX);
                    newCell.FlipY.Should().Be(oldCell.FlipY);
                    newCell.SelectParam.Should().Be(oldCell.SelectParam);
                    //newCell.TileOffset.Should().Be(oldCell.TileOffset); // the tile offset is not always the same because
                                                                          // we don't do as much optimisation as they do
                                                                          // e.g. sometimes they share identical pixel buffers
                    newCell.Priority.Should().Be(oldCell.Priority);
                    
                }
            }

            if (checkClusterMinMax)
            {
                using (new AssertionScope())
                {
                    // How is ReadOnlyCellInfo generated? Still needs to be figured out, but anims seem to work fine without it.
                    //newCluster.ReadOnlyCellInfo.Should().Be(oldCluster.ReadOnlyCellInfo);
                    newCluster.XMin.Should().Be(oldCluster.XMin);
                    newCluster.XMax.Should().Be(oldCluster.XMax);
                    newCluster.YMin.Should().Be(oldCluster.YMin);
                    newCluster.YMax.Should().Be(oldCluster.YMax);
                }
            }
        }
    }
}