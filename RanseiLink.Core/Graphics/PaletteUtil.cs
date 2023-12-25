using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Graphics;

public static class PaletteUtil
{
    public static Rgba32 To32BitColor(Rgb15 color)
    {
        return new Rgba32((byte)(color.R * 8), (byte)(color.G * 8), (byte)(color.B * 8));
    }
    public static Rgba32[] To32bitColors(IEnumerable<Rgb15> colors)
    {
        return colors.Select(To32BitColor).ToArray();
    }

    public static Rgb15 From32BitColor(Rgba32 color)
    {
        return new Rgb15(color.R / 8, color.G / 8, color.B / 8);
    }

    public static Rgb15[] From32bitColors(IEnumerable<Rgba32> colors)
    {
        return colors.Select(From32BitColor).ToArray();
    }

    public static Rgb15[] Decompress(byte[] data)
    {
        return data.ToUInt16Array().Select(i => Rgb15.From(i)).ToArray();
    }

    public static byte[] Compress(Rgb15[] colors)
    {
        return colors.Select(i => i.ToUInt16()).ToArray().ToByteArray();
    }

    public static void SaveAsPaintNetPalette(Rgba32[] colors, string file)
    {
        using (var sw = new StreamWriter(File.Create(file)))
        {
            foreach (var col in colors)
            {
                sw.WriteLine($"{col.A:X2}{col.R:X2}{col.G:X2}{col.B:X2}");
            }
        } 
    }
}