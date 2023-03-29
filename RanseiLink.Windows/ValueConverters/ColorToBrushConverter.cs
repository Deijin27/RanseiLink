using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class ColorToBrushConverter : ValueConverter<Color, SolidColorBrush>
{
    protected override SolidColorBrush Convert(Color value)
    {
        Color c = Color.FromArgb(value.A, value.R, value.G, value.B);
        return new SolidColorBrush(c);
    }

    protected override Color ConvertBack(SolidColorBrush value)
    {
        return Color.FromArgb(value.Color.A, value.Color.R, value.Color.G, value.Color.B);
    }
}
