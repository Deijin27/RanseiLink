using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RanseiLink.Core.Graphics;

public static class ImageUtil
{
    public static void SpriteToPng(string file, SpriteImageInfo imageInfo)
    {
        using (var img = SpriteToImage(imageInfo))
        {
            img.SaveAsPng(file);
        }
    }

    public static Image<Rgba32> SpriteToImage(SpriteImageInfo imageInfo)
    {
        if (imageInfo.Width <= 0 || imageInfo.Height <= 0)
        {
            throw new ArgumentException($"Image width and height provied to {nameof(SpriteToImage)} must be greater than zero");
        }
        var pointGetter = PointUtil.DecidePointGetter(imageInfo.IsTiled);
        var img = new Image<Rgba32>(imageInfo.Width, imageInfo.Height);

        switch (imageInfo.Format)
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
                throw new Exception($"Invalid tex format {imageInfo.Format}");
        }

        return img;
    }

    public static SpriteImageInfo SpriteFromPng(string file, bool tiled, TexFormat format, bool color0ToTransparent)
    {
        try
        {
            using var image = LoadPngBetterError(file);
            return SpriteFromImage(image, tiled, format, color0ToTransparent);
        }
        catch (Exception e)
        {
            throw new Exception($"Error converting image '{file}'", e);
        }
    }

    public static SpriteImageInfo SpriteFromImage(Image<Rgba32> image, bool tiled, TexFormat format, bool color0ToTransparent)
    {
        int width = image.Width;
        int height = image.Height;
        var palette = new Palette(format, color0ToTransparent);

        byte[] pixels = SharedPalettePixelsFromImage(image, palette, tiled, format, color0ToTransparent);

        return new SpriteImageInfo(pixels, palette, width, height, tiled, format);
    }

    public static byte[] SharedPalettePixelsFromImage(Image<Rgba32> image, Palette palette, bool tiled, TexFormat format, bool color0ToTransparent)
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
        catch (Exception e)
        {
            throw new Exception($"Error loading image file '{file}'", e);
        }
        return image;
    }

    public static (bool hasTransparency, bool hasSemiTransparency) ImageHasTransparency(Image<Rgba32> image)
    {
        bool hasTransparency = false;
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var a = image[x, y].A;
                if (a != 255)
                {
                    if (a != 0)
                    {
                        return (true, true);
                    }
                    else
                    {
                        hasTransparency = true;
                    }
                }
            }
        }
        return (hasTransparency, false);
    }

    /// <summary>
    /// Creates a new images that is the <paramref name="images"/> vertically stacked.
    /// The width of the resulting images is the same as the maximum of the constituent images.
    /// </summary>
    public static Image<TPixel> CombineImagesVertically<TPixel>(IReadOnlyCollection<Image<TPixel>> images) where TPixel : unmanaged, IPixel<TPixel>
    {
        int maxWidth = 0;
        int totalHeight = 0;
        foreach (var image in images)
        {
            maxWidth = Math.Max(maxWidth, image.Width);
            totalHeight += image.Height;
        }

        var combinedImage = new Image<TPixel>(maxWidth, totalHeight);

        combinedImage.Mutate(g =>
        {
            var cumulativeHeight = 0;
            foreach (var image in images)
            {
                g.DrawImage(
                    image,
                    new Point(0, cumulativeHeight),
                    1
                    );
                cumulativeHeight += image.Height;
            }
        });
        return combinedImage;
    }

    public static Image<TPixel> Crop<TPixel>(Image<TPixel> image, Rectangle cropRectangle) where TPixel : unmanaged, IPixel<TPixel>
    {
        return image.Clone(g => g.Crop(cropRectangle));
    }


    /// <summary>
    /// Safely crop an image. If a portion of the cropRectangle is outside of the image bounds, 
    /// that portion of the resulting image will be transparent.
    /// </summary>
    public static Image<TPixel> SafeCrop<TPixel>(Image<TPixel> image, Rectangle cropRectangle) where TPixel : unmanaged, IPixel<TPixel>
    {
        if (cropRectangle.X >= 0 && cropRectangle.Y >= 0 && cropRectangle.Right <= image.Width && cropRectangle.Bottom <= image.Height)
        {
            // the crop rectange is in the boundaries of the image so
            // it's safe to do a regular crop
            return image.Clone(g => g.Crop(cropRectangle));
        }

        int safeX = Math.Max(cropRectangle.X, 0);
        int safeY = Math.Max(cropRectangle.Y, 0);
        int safeR = Math.Min(cropRectangle.Right, image.Width);
        var safeB = Math.Min(cropRectangle.Bottom, image.Height);
        var safeW = safeR - safeX;
        var safeH = safeB - safeY;
        var resultImg = new Image<TPixel>(cropRectangle.Width, cropRectangle.Height);
        if (safeW <= 0 || safeH <= 0)
        {
            // the image is completely outside of the bounds
            // so we just create a new blank image with the correct dims
            return resultImg;
        }
        // safely crop a section of the image then draw it onto
        // a correctly sized image at the right position
        using var cropped = image.Clone(g => g.Crop(new Rectangle(safeX, safeY, safeW, safeH)));
        resultImg.Mutate(g => g.DrawImage(cropped, new Point(safeX - cropRectangle.X, safeY - cropRectangle.Y), opacity: 1));
        return resultImg;
    }
}
