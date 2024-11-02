using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class ImageSharpColorToBrushConverter : ValueConverter<Rgba32, SolidColorBrush>
{
    protected override SolidColorBrush Convert(Rgba32 value)
    {
        Color c = Color.FromArgb(value.A, value.R, value.G, value.B);
        return new SolidColorBrush(c);
    }

    protected override Rgba32 ConvertBack(SolidColorBrush value)
    {
        return new Rgba32(value.Color.R, value.Color.G, value.Color.B, value.Color.A);
    }
}
