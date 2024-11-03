using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Util;
public static class ColorUtil
{
    public static Rgba32 ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)value;
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => new Rgba32(v, t, p),
            1 => new Rgba32(q, v, p),
            2 => new Rgba32(p, v, t),
            3 => new Rgba32(p, q, v),
            4 => new Rgba32(t, p, v),
            _ => new Rgba32(v, p, q)
        };
    }
}
