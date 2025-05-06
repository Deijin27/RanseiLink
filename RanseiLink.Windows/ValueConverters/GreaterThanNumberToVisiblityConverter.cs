using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RanseiLink.Windows.ValueConverters;

public class GreaterThanNumberToVisiblityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dValue = System.Convert.ToDouble(value);
        var dParameter = System.Convert.ToDouble(parameter);
        if (dValue > dParameter)
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}