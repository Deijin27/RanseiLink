using System.Globalization;
using System.Windows.Data;

namespace RanseiLink.Windows.ValueConverters;

public class EnumDisplayValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        if (!value.GetType().IsEnum)
        {
            return value;
        }
        var name = Enum.GetName(value.GetType(), value);
        var num = System.Convert.ToInt32(value);
        return $"{num:000} - {name}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
