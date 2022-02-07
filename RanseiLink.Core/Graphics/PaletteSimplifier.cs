using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
    /// <param name="imagePath"></param>
    /// <returns>False if the paletted didn't need to be simplified</returns>
    public static bool SimplifyPalette(string imagePath, int maximumColors)
    {
        // load image
        using var img = Image.Load<Rgba32>(imagePath);

        // Create a lookup of colors to points
        var groupByColor = new Dictionary<Rgba32, List<Point>>();
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var color = img[x, y];
                if (color.A == 0)
                {
                    continue;
                }
                if (!groupByColor.TryGetValue(color, out var lst))
                {
                    lst = new List<Point>();
                    groupByColor[color] = lst;
                }
                lst.Add(new Point(x, y));   
            }
        }

        // if not using too many colors, no need to simplify
        if (groupByColor.Count + 1 <= maximumColors)
        {
            return false;
        }

        // simplify palette
        while (groupByColor.Count + 1 > maximumColors)
        {
            // find the minimum difference between colors
            int minDistance = int.MaxValue;
            Rgba32 minColor1 = default;
            Rgba32 minColor2 = default;
            foreach (var color1 in groupByColor.Keys)
            {
                foreach (var color2 in groupByColor.Keys)
                {
                    if (color2 != color1)
                    {
                        int dist = Distance(color1, color2);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            minColor1 = color1;
                            minColor2 = color2;
                        }
                    }
                    
                }
            }

            // move the nearest colors into a shared group of their average
            var lst = groupByColor[minColor1];
            lst.AddRange(groupByColor[minColor2]);
            groupByColor.Remove(minColor2);
            groupByColor.Remove(minColor1);
            groupByColor[Average(minColor1, minColor2)] = lst;
        }

        // set colors of image to new palette
        foreach (var (color, pointList) in groupByColor)
        {
            foreach (Point point in pointList)
            {
                img[point.X, point.Y] = color;
            }
        }

        // save image
        string saveFile = FileUtil.MakeUniquePath(Path.Combine(
            Path.GetDirectoryName(imagePath), Path.GetFileNameWithoutExtension(imagePath) + " - Simplified" + Path.GetExtension(imagePath)
            ));
        img.Save(saveFile);

        return true;
    }

    private static int Distance(Rgba32 color1, Rgba32 color2)
    {
        return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
    }

    private static Rgba32 Average(Rgba32 color1, Rgba32 color2)
    {
        return new Rgba32(
            (byte)((color1.R + color2.R) / 2),
            (byte)((color1.G + color2.G) / 2),
            (byte)((color1.B + color2.B) / 2),
            (byte)((color1.A + color2.A) / 2)
            );
    }
}
