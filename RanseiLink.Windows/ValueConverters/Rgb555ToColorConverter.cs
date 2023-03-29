using RanseiLink.Core.Graphics;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class Rgb555ToColorConverter : ValueConverter<Rgb15, Color>
{
    private static byte ConvertComponent(int rgbaComponent)
    {
        double c = rgbaComponent;
        return (byte)(Math.Round(c * 255 / 31));
    }

    private static int ConvertComponentBack(byte colorComponent)
    {
        double c = colorComponent;
        return (byte)(Math.Round(c * 31 / 255));
    }

    protected override Color Convert(Rgb15 value)
    {
        return Color.FromRgb(ConvertComponent(value.R), ConvertComponent(value.G), ConvertComponent(value.B));
    }

    protected override Rgb15 ConvertBack(Color value)
    {
        return new(ConvertComponentBack(value.R), ConvertComponentBack(value.G), ConvertComponentBack(value.B));
    }
}

public class Rgb555ToBrushConverter : ValueConverter<Rgb15, Brush>
{
    private static readonly IValueConverter _rgba5555ToColorConverter = new Rgb555ToColorConverter();
    protected override Brush Convert(Rgb15 value)
    {
        return new SolidColorBrush((Color)_rgba5555ToColorConverter.Convert(value, typeof(Color), null, null));
    }

    protected override Rgb15 ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}