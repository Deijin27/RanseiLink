using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class ZToHueConverter : ValueConverter<float, Brush>
{
    private const double _value = 0.6;
    private const double _saturation = 0.6;

    private static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = System.Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)value;
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => Color.FromRgb(v, t, p),
            1 => Color.FromRgb(q, v, p),
            2 => Color.FromRgb(p, v, t),
            3 => Color.FromRgb(p, q, v),
            4 => Color.FromRgb(t, p, v),
            _ => Color.FromRgb(v, p, q)
        };
    }

    public Brush ConvertValue(float value)
    {
        return Convert(value);
    }

    protected override Brush Convert(float value)
    {
        return new SolidColorBrush(ColorFromHSV((double)value / 40 / 25 * 255, _saturation, _value));
    }

    protected override float ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }

    
}
