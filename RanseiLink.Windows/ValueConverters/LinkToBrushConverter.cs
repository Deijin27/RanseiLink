using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class LinkToBrushConverter : ValueConverter<int, SolidColorBrush>
{
    protected override SolidColorBrush Convert(int value)
    {
        if (value < 50 || value > 100)
        {
            return Brushes.Transparent;
        }
        else if (value >= 90)
        {
            return Brushes.Gold;
        }
        else if (value >= 70)
        {
            return Brushes.Silver;
        }
        else
        {
            return Brushes.RosyBrown;
        }
    }

    protected override int ConvertBack(SolidColorBrush value)
    {
        throw new NotImplementedException();
    }
}
