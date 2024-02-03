using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Graphics;

public static class PaletteUtil
{
    public static Rgba32 To32BitColor(Rgb15 color)
    {
        return new Rgba32((byte)(color.R * 8), (byte)(color.G * 8), (byte)(color.B * 8));
    }

    public static Rgba32[] To32bitColors(IReadOnlyCollection<Rgb15> colors)
    {
        var result = new Rgba32[colors.Count];
        int count = 0;
        foreach (var color in colors)
        {
            result[count++] = To32BitColor(color);
        }
        return result;
    }

    public static Rgb15 From32BitColor(Rgba32 color)
    {
        return new Rgb15(color.R / 8, color.G / 8, color.B / 8);
    }

    public static Rgb15[] From32bitColors(IReadOnlyCollection<Rgba32> colors)
    {
        var result = new Rgb15[colors.Count];
        int count = 0;
        foreach (var color in colors)
        {
            result[count++] = From32BitColor(color);
        }
        return result;
    }

    public static Rgb15[] Decompress(byte[] data)
    {
        var ushorts = data.ToUInt16Array();
        var result = new Rgb15[ushorts.Length];
        for (int i = 0; i < ushorts.Length; i++)
        {
            result[i] = Rgb15.From(ushorts[i]);
        }
        return result;
    }

    public static byte[] Compress(Rgb15[] colors)
    {
        ushort[] result = new ushort[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            result[i] = colors[i].ToUInt16();
        }
        return result.ToByteArray();
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