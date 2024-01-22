using System.Windows;

namespace RanseiLink.Windows.ValueConverters;

public class GreaterThanNumberToVisiblityConverter : ValueConverter<int, Visibility>
{
    public int Number { get; set; } = 0;

    protected override Visibility Convert(int value)
    {
        return value > Number ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override int ConvertBack(Visibility value)
    {
        throw new NotImplementedException();
    }
}