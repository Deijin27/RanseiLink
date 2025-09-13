using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RanseiLink.XP.ValueConverters;

public abstract class ValueConverter<TSource, TTarget> : IValueConverter
{
    protected abstract TTarget Convert(TSource value);

    protected abstract TSource ConvertBack(TTarget value);

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TSource))
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }
        return Convert((TSource)value);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }
        return ConvertBack((TTarget)value);
    }
}