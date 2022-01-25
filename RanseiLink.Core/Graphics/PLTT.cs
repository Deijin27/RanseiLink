using SixLabors.ImageSharp.PixelFormats;
using System.Linq;

namespace RanseiLink.Core.Graphics;

public static class PLTT
{
    public static Rgba32[] To32bitColors(Rgb15[] colors)
    {
        return colors.Select(color => new Rgba32((byte)(color.R * 8), (byte)(color.G * 8), (byte)(color.B * 8))).ToArray();
    }

    public static Rgb15[] From32bitColors(Rgba32[] colors)
    {
        return colors.Select(color => new Rgb15(color.R / 8, color.G / 8, color.B / 8)).ToArray();
    }

    public static Rgb15[] Decompress(byte[] data)
    {
        return data.ToUInt16Array().Select(i => Rgb15.From(i)).ToArray();
    }

    public static byte[] Compress(Rgb15[] colors)
    {
        return colors.Select(i => i.ToUInt16()).ToArray().ToByteArray();
    }
}
