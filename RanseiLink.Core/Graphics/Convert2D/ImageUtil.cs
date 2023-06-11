using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace RanseiLink.Core.Graphics;

public static class ImageUtil
{
    public static void SpriteToPng(string file, SpriteImageInfo imageInfo, bool tiled, TexFormat format)
    {
        using (var img = SpriteToImage(imageInfo, tiled, format))
        {
            img.SaveAsPng(file);
        }
    }

    public static Image<Rgba32> SpriteToImage(SpriteImageInfo imageInfo, bool tiled, TexFormat format)
    {
        var pointGetter = PointUtil.DecidePointGetter(tiled);
        var img = new Image<Rgba32>(imageInfo.Width, imageInfo.Height);

        switch (format)
        {
            case TexFormat.Pltt4:
            case TexFormat.Pltt16:
            case TexFormat.Pltt256:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        color = imageInfo.Palette[pix];

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.A3I5:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        int colorIndex = pix & 0b11111;
                        int alphaSrc = pix >> 5;
                        byte alpha = (byte)((alphaSrc * 4 + alphaSrc / 2) * 8);
                        var baseColor = imageInfo.Palette[colorIndex];
                        color = new Rgba32(baseColor.R, baseColor.G, baseColor.B, alpha);

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.A5I3:
                {
                    for (int i = 0; i < imageInfo.Pixels.Length; i++)
                    {
                        Point point = pointGetter(i, imageInfo.Width);
                        Rgba32 color;

                        byte pix = imageInfo.Pixels[i];
                        int colorIndex = pix & 0b111;
                        byte alpha = (byte)((pix >> 3) * 8);
                        var baseColor = imageInfo.Palette[colorIndex];
                        color = new Rgba32(baseColor.R, baseColor.G, baseColor.B, alpha);

                        img[point.X, point.Y] = color;
                    }
                    
                    break;
                }
            case TexFormat.Direct:
            case TexFormat.None:
            case TexFormat.Comp4x4:
            default:
                throw new Exception($"Invalid tex format {format}");
        }

        return img;
    }

    public static SpriteImageInfo SpriteFromPng(string file, bool tiled, TexFormat format, bool color0ToTransparent)
    {
        using var image = LoadPngBetterError(file);

        int width = image.Width;
        int height = image.Height;
        var palette = new List<Rgba32>();
        if (color0ToTransparent)
        {
            palette.Add(Color.Transparent);
        }

        byte[] pixels;
        try
        {
            pixels = SharedPalettePixelsFromImage(image, palette, tiled, format, color0ToTransparent);
        }
        catch (Exception e)
        {
            throw new Exception($"Error converting image '{file}'", e);
        }

        return new SpriteImageInfo(pixels, palette.ToArray(), width, height);
    }

    public static byte[] SharedPalettePixelsFromImage(Image<Rgba32> image, List<Rgba32> palette, bool tiled, TexFormat format, bool color0ToTransparent)
    {
        var indexGetter = PointUtil.DecideIndexGetter(tiled);
        int width = image.Width;
        int height = image.Height;
        byte[] pixels = new byte[width * height];

        switch (format)
        {
            case TexFormat.Pltt4:
            case TexFormat.Pltt16:
            case TexFormat.Pltt256:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            if (color0ToTransparent && pixColor.A != 255)
                            {
                                pltIndex = 0;
                            }
                            else
                            {
                                pixColor.A = 255;
                                pltIndex = palette.IndexOf(pixColor);
                                if (pltIndex == -1)
                                {
                                    pltIndex = palette.Count;
                                    palette.Add(pixColor);
                                }
                                if (pltIndex > byte.MaxValue)
                                {
                                    throw new InvalidPaletteException($"There can not be more than {byte.MaxValue + 1} colors");
                                }
                            }

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    break;
                }
            case TexFormat.A3I5:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            var alphaColor = pixColor.A;
                            pixColor.A = 255;
                            pltIndex = palette.IndexOf(pixColor);
                            if (pltIndex == -1)
                            {
                                pltIndex = palette.Count;
                                palette.Add(pixColor);
                            }
                            //if (pltIndex > 31)
                            //{
                            //    throw new InvalidPaletteException($"There can not be more than 32 colors");
                            //}
                            var alpha = Math.Min(7, alphaColor * 8 / 255); // this accurately reproduces the values that are created the other way, even if it's weird.
                            pltIndex = alpha << 5 | pltIndex;

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    if (palette.Count > 32)
                    {
                        throw new InvalidPaletteException($"There can not be more than 32 colors (there are {palette.Count} colors)");
                    }
                    break;
                }
            case TexFormat.A5I3:
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Rgba32 pixColor = image[x, y];
                            int pltIndex;

                            var alphaColor = pixColor.A;
                            pixColor.A = 255;
                            pltIndex = palette.IndexOf(pixColor);
                            if (pltIndex == -1)
                            {
                                pltIndex = palette.Count;
                                palette.Add(pixColor);
                            }
                            if (pltIndex > 7)
                            {
                                throw new InvalidPaletteException($"There can not be more than 8 colors");
                            }
                            var alpha = alphaColor / 8;
                            pltIndex = alpha << 3 | pltIndex;

                            pixels[indexGetter(new Point(x, y), width)] = (byte)pltIndex;
                        }
                    }
                    
                    break;
                }
            case TexFormat.Direct:
            case TexFormat.None:
            case TexFormat.Comp4x4:
                throw new Exception($"Invalid tex format {format}");
        }

        return pixels;
    }

    public static Image<Rgba32> LoadPngBetterError(string file)
    {
        Image<Rgba32> image;
        try
        {
            image = Image.Load<Rgba32>(file);
        }
        catch (UnknownImageFormatException e)
        {
            throw new UnknownImageFormatException(e.Message + $" File='{file}'");
        }
        return image;
    }
}
