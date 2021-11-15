using System;
using System.Globalization;
using System.Windows.Data;

namespace RanseiLink.ValueConverters;
#region Value Converter

public abstract class ValueConverter<TSource, TTarget> : IValueConverter
{
    protected abstract TTarget Convert(TSource value);

    protected abstract TSource ConvertBack(TTarget value);

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TSource))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TSource).FullName}.");
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

public abstract class ValueConverter<TSource, TTarget, TParameter> : IValueConverter
{
    protected abstract TTarget Convert(TSource value, TParameter parameter);

    protected abstract TSource ConvertBack(TTarget value, TParameter parameter);

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TSource))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TSource).FullName}.");
        }
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }
        return Convert((TSource)value, (TParameter)parameter);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }
        return ConvertBack((TTarget)value, (TParameter)parameter);
    }
}

#endregion

#region Array MultiValueConverter

public abstract class MultiValueConverter<TSource, TTarget> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource[] values);

    protected abstract TSource[] ConvertBack(TTarget value);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // First do a check to stop exceptions in design view
        if (values[0] == System.Windows.DependencyProperty.UnsetValue)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }

        TSource[] castValues = Array.ConvertAll(values, value =>
        {
            if (value != null && !(value is TSource))
            {
                throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
            }
            return (TSource)value;
        });

        return Convert(castValues);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }

        TSource[] results = ConvertBack((TTarget)value);

        return Array.ConvertAll<TSource, object>(results, i => i);
    }
}

public abstract class ParameterMultiValueConverter<TSource, TTarget, TParameter> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource[] values, TParameter parameter);

    protected abstract TSource[] ConvertBack(TTarget value, TParameter parameter);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }
        TSource[] castValues = Array.ConvertAll(values, value =>
        {
            if (value != null && !(value is TSource))
            {
                throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
            }
            return (TSource)value;
        });

        return Convert(castValues, (TParameter)parameter);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }

        TSource[] results = ConvertBack((TTarget)value, (TParameter)parameter);

        return Array.ConvertAll<TSource, object>(results, i => i);
    }
}

#endregion

#region Two Item MultiValueConverter

public abstract class MultiValueConverter<TSource0, TSource1, TTarget> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource0 value0, TSource1 value1);

    protected abstract (TSource0 value0, TSource1 value1) ConvertBack(TTarget value);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
        {
            throw new ArgumentException($"In {GetType().FullName} {nameof(Convert)}, the number of values should be 2, but was {values.Length}.");
        }
        object value0 = values[0];
        if (value0 != null && !(value0 is TSource0))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value0 of type {value0.GetType().FullName} to type of {typeof(TSource0).FullName}.");
        }
        object value1 = values[1];
        if (value1 != null && !(value1 is TSource1))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value1 of type {value1.GetType().FullName} to type of {typeof(TSource1).FullName}.");
        }

        return Convert((TSource0)value0, (TSource1)value1);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }

        var (value0, value1) = ConvertBack((TTarget)value);

        return new object[] { value0, value1 };
    }
}

public abstract class ParameterMultiValueConverter<TSource0, TSource1, TTarget, TParameter> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource0 value0, TSource1 value1, TParameter parameter);

    protected abstract (TSource0 value0, TSource1 value1) ConvertBack(TTarget value, TParameter parameter);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }
        if (values.Length != 2)
        {
            throw new ArgumentException($"In {GetType().FullName} {nameof(Convert)}, the number of values should be 2, but was {values.Length}.");
        }
        object value0 = values[0];
        if (value0 != null && !(value0 is TSource0))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value0 of type {value0.GetType().FullName} to type of {typeof(TSource0).FullName}.");
        }
        object value1 = values[1];
        if (value1 != null && !(value1 is TSource1))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value1 of type {value1.GetType().FullName} to type of {typeof(TSource1).FullName}.");
        }

        return Convert((TSource0)value0, (TSource1)value1, (TParameter)parameter);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }

        var (value0, value1) = ConvertBack((TTarget)value, (TParameter)parameter);

        return new object[] { value0, value1 };
    }
}

#endregion

#region Three Item MultiValueConverter

public abstract class MultiValueConverter<TSource0, TSource1, TSource2, TTarget> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource0 value0, TSource1 value1, TSource2 value2);

    protected abstract (TSource0 value0, TSource1 value1, TSource2 value2) ConvertBack(TTarget value);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3)
        {
            throw new ArgumentException($"In {GetType().FullName} {nameof(Convert)}, the number of values should be 3, but was {values.Length}.");
        }
        object value0 = values[0];
        if (value0 != null && !(value0 is TSource0))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value0 of type {value0.GetType().FullName} to type of {typeof(TSource0).FullName}.");
        }
        object value1 = values[1];
        if (value1 != null && !(value1 is TSource1))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value1 of type {value1.GetType().FullName} to type of {typeof(TSource1).FullName}.");
        }
        object value2 = values[2];
        if (value2 != null && !(value2 is TSource2))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value2 of type {value1.GetType().FullName} to type of {typeof(TSource2).FullName}.");
        }

        return Convert((TSource0)value0, (TSource1)value1, (TSource2)value2);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }

        var (value0, value1, value2) = ConvertBack((TTarget)value);

        return new object[] { value2, value1, value2 };
    }
}

public abstract class ParameterMultiValueConverter<TSource0, TSource1, TSource2, TTarget, TParameter> : IMultiValueConverter
{
    protected abstract TTarget Convert(TSource0 value0, TSource1 value1, TSource2 value2, TParameter parameter);

    protected abstract (TSource0 value0, TSource1 value1, TSource2 value2) ConvertBack(TTarget value, TParameter parameter);

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }
        if (values.Length != 3)
        {
            throw new ArgumentException($"In {GetType().FullName} {nameof(Convert)}, the number of values should be 3, but was {values.Length}.");
        }
        object value0 = values[0];
        if (value0 != null && !(value0 is TSource0))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value0 of type {value0.GetType().FullName} to type of {typeof(TSource0).FullName}.");
        }
        object value1 = values[1];
        if (value1 != null && !(value1 is TSource1))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value1 of type {value1.GetType().FullName} to type of {typeof(TSource1).FullName}.");
        }
        object value2 = values[2];
        if (value2 != null && !(value2 is TSource2))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(Convert)}, unable cast value2 of type {value1.GetType().FullName} to type of {typeof(TSource2).FullName}.");
        }

        return Convert((TSource0)value0, (TSource1)value1, (TSource2)value2, (TParameter)parameter);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value != null && !(value is TTarget))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast value of type {value.GetType().FullName} to type of {typeof(TTarget).FullName}.");
        }
        if (parameter != null && !(parameter is TParameter))
        {
            throw new InvalidCastException($"In {GetType().FullName} {nameof(ConvertBack)}, unable cast parameter of type {parameter.GetType().FullName} to type of {typeof(TParameter).FullName}.");
        }

        var (value0, value1, value2) = ConvertBack((TTarget)value, (TParameter)parameter);

        return new object[] { value0, value1, value2 };
    }
}

#endregion