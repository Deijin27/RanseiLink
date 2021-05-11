using System;
using System.Globalization;
using System.Windows.Data;

namespace RanseiWpf.ValueConverters
{
    public abstract class ValueConverter<TSource, TTarget> : IValueConverter
    {
        protected abstract TTarget Convert(TSource value);

        protected abstract TSource ConvertBack(TTarget value);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(value is TSource))
            {
                throw new InvalidCastException(nameof(Convert));
            }
            return Convert((TSource)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(value is TTarget))
            {
                throw new InvalidCastException(nameof(ConvertBack));
            }
            return ConvertBack((TTarget)value);
        }
    }
}
