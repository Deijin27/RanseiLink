using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Graphics;

public static class ImageSimplifier
{
    /// <summary>
    /// Reduce the number of colors in an image with quantization.
    /// Saves to a file next to the input file.
    /// </summary>
    /// <param name="imagePath">Path of image file to simplify</param>
    /// <param name="maximumColors">Palette capacity of image</param>
    /// <returns>True if the image was simplified. False if simplification was not necessary</returns>
    /// <exception cref="UnknownImageFormatException"></exception>
    public static bool SimplifyPalette(string imagePath, int maximumColors)
    {
        string saveFile = FileUtil.MakeUniquePath(Path.Combine(
            Path.GetDirectoryName(imagePath)!, Path.GetFileNameWithoutExtension(imagePath) + " - Simplified" + Path.GetExtension(imagePath)
            ));

        return SimplifyPalette(imagePath, maximumColors, saveFile);
    }

    /// <summary>
    /// Reduce the number of colors in an image with quantization
    /// </summary>
    /// <param name="imagePath">Path of image file to simplify</param>
    /// <param name="maximumColors">Palette capacity of image</param>
    /// <param name="saveFile">File to save simplified version of image to</param>
    /// <returns>True if the image was simplified. False if simplification was not necessary</returns>
    /// <exception cref="UnknownImageFormatException"></exception>
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

        var transparentPixelsBefore = new List<Point>();
        // Create a lookup of colors to points
        var colors = new HashSet<Rgba32>();
        bool hasTransparent = false;
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var color = img[x, y];
                if (color.A != 255)
                {
                    hasTransparent = true;
                    transparentPixelsBefore.Add(new Point(x, y));
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

        if (!hasTransparent)
        {
            maximumColors -= 1;
        }
        System.Console.WriteLine($"Max colors: {maximumColors}");

        img.Mutate(g =>
        {
            g.Quantize(new OctreeQuantizer(new QuantizerOptions() { MaxColors = maximumColors, DitherScale = 0 }));
        });

        // Check the color count after quanitization. If transparency is not preserved
        // (happens usually if theres a black pixel), we've got a problem
        colors.Clear();
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var color = img[x, y];
                if (color.A != 255)
                {
                    continue;
                }
                colors.Add(color);
            }
        }
        if (colors.Count + 1 > maximumColors)
        {
            System.Console.WriteLine($"Doing second quanitization with color limit {maximumColors - 1}");
            // quantize again, and allow for the required transparency.
            img.Mutate(g =>
            {
                g.Quantize(new OctreeQuantizer(new QuantizerOptions() { MaxColors = maximumColors - 1, DitherScale = 0 }));
            });
            // Add in the transparent pixels after to make sure they are preserved.
            if (hasTransparent)
            {
                foreach (var pixel in transparentPixelsBefore)
                {
                    img[pixel.X, pixel.Y] = Color.Transparent;
                }
            }
        }


        img.Save(saveFile);

        img.Dispose();

        return true;
    }

    public static bool ImageMatchesSize(string imagePath, int width, int height)
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

        bool result = img.Height == height && img.Width == width;

        img.Dispose();

        return result;
    }

    public static void ResizeImage(string imagePath, int width, int height, string saveFile)
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

        img.Mutate(g =>
        {
            g.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            });
        });

        img.Save(saveFile);

        img.Dispose();
    }
}