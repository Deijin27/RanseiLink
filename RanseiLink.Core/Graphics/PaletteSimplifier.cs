using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public static class PaletteSimplifier
{
    /// <summary>
    /// Simplify palette by grouping colors nearest to eachother
    /// </summary>
    /// <returns>False if the paletted didn't need to be simplified</returns>
    public static bool SimplifyPalette(string imagePath, int maximumColors)
    {
        string saveFile = FileUtil.MakeUniquePath(Path.Combine(
            Path.GetDirectoryName(imagePath), Path.GetFileNameWithoutExtension(imagePath) + " - Simplified" + Path.GetExtension(imagePath)
            ));

        return SimplifyPalette(imagePath, maximumColors, saveFile);
    }

    public static bool SimplifyPalette(string imagePath, int maximumColors, string saveFile)
    {
        Image<Rgba32> img;
        try
        {
            img = Image.Load<Rgba32>(imagePath);
        }
        catch (UnknownImageFormatException e)
        {
            throw new UnknownImageFormatException(e.Message + $" File='{imagePath}'");
        }

        // Create a lookup of colors to points
        var colors = new HashSet<Rgba32>();
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var color = img[x, y];
                if (color.A == 0)
                {
                    continue;
                }
                colors.Add(color);
            }
        }
        // if not using too many colors, no need to simplify
        if (colors.Count + 1 <= maximumColors)
        {
            return false;
        }

        img.Mutate(g =>
        {
            g.Quantize(new OctreeQuantizer(new QuantizerOptions() { MaxColors = maximumColors, DitherScale = 0 }));
        });

        img.Save(saveFile);

        return true;
    }
}
