using System.Windows;

namespace RanseiLink.ValueConverters;

internal class InvertBoolToVisibilityConverter : ValueConverter<bool, Visibility>
{
    protected override Visibility Convert(bool value)
    {
        return value ? Visibility.Collapsed : Visibility.Visible;
    }

    protected override bool ConvertBack(Visibility value)
    {
        return value != Visibility.Visible;
    }
}
