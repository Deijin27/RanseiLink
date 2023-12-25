using System.Windows;

namespace RanseiLink.Windows.ValueConverters;

public class GreaterThanZeroToVisiblityConverter : ValueConverter<int, Visibility>
{
    protected override Visibility Convert(int value)
    {
        return value > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override int ConvertBack(Visibility value)
    {
        throw new NotImplementedException();
    }
}
