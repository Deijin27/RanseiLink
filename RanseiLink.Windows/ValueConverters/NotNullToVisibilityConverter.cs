using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RanseiLink.Windows.ValueConverters;

public class NotNullToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; } = false;
    public bool Hidden { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var notnull = value != null && (value is not string strval || strval != string.Empty);
        if (Inverse)
        {
            notnull = !notnull;
        }
        if (notnull)
        {
            return Visibility.Visible;
        }
        else
        {
            if (Hidden)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}