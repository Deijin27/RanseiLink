using RanseiLink.Core.Types;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace RanseiLink.ValueConverters;

public class Rgb555ToColorConverter : ValueConverter<Rgb555, Color>
{
    private static byte ConvertComponent(uint rgbaComponent)
    {
        double c = rgbaComponent;
        return (byte)(Math.Round(c * 255 / 31));
    }

    private static uint ConvertComponentBack(byte colorComponent)
    {
        double c = colorComponent;
        return (byte)(Math.Round(c * 31 / 255));
    }

    protected override Color Convert(Rgb555 value)
    {
        return Color.FromRgb(ConvertComponent(value.R), ConvertComponent(value.G), ConvertComponent(value.B));
    }

    protected override Rgb555 ConvertBack(Color value)
    {
        return new(ConvertComponentBack(value.R), ConvertComponentBack(value.G), ConvertComponentBack(value.B));
    }
}

public class Rgb555ToBrushConverter : ValueConverter<Rgb555, Brush>
{
    private static readonly IValueConverter _rgba5555ToColorConverter = new Rgb555ToColorConverter();
    protected override Brush Convert(Rgb555 value)
    {
        return new SolidColorBrush((Color)_rgba5555ToColorConverter.Convert(value, typeof(Color), null, null));
    }

    protected override Rgb555 ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}