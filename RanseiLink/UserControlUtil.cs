using System;
using System.Windows;
using System.Linq.Expressions;

namespace RanseiLink;

public static class UserControlUtil
{
    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property,
        TProperty defaultValue, PropertyChangedCallback<TView, TProperty> propertyChangedCallback)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        PropertyChangedCallback callback = (d, e) =>
        {
            DependencyPropertyChangedEventArgs<TProperty> eArgs =
            new DependencyPropertyChangedEventArgs<TProperty>(e.Property, (TProperty)e.OldValue, (TProperty)e.NewValue);

            propertyChangedCallback?.Invoke((TView)d, eArgs);
        };

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new PropertyMetadata(defaultValue, callback)
            );
    }

    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property,
        TProperty defaultValue, FrameworkPropertyMetadataOptions options, PropertyChangedCallback<TView, TProperty> propertyChangedCallback)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        PropertyChangedCallback callback = (d, e) =>
        {
            DependencyPropertyChangedEventArgs<TProperty> eArgs =
            new DependencyPropertyChangedEventArgs<TProperty>(e.Property, (TProperty)e.OldValue, (TProperty)e.NewValue);

            propertyChangedCallback?.Invoke((TView)d, eArgs);
        };

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new FrameworkPropertyMetadata(defaultValue, options, callback)
            );
    }

    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property,
        TProperty defaultValue, FrameworkPropertyMetadataOptions options)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new FrameworkPropertyMetadata(defaultValue, options)
            );
    }

    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property,
        TProperty defaultValue)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new PropertyMetadata(defaultValue)
            );
    }

    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new PropertyMetadata(default(TProperty))
            );
    }

    public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
        Expression<Func<TView, TProperty>> property,
        PropertyChangedCallback<TView, TProperty> propertyChangedCallback)
        where TView : DependencyObject
    {
        var expression = (MemberExpression)property.Body;
        var propertyName = expression.Member.Name;

        PropertyChangedCallback callback = (d, e) =>
        {
            DependencyPropertyChangedEventArgs<TProperty> eArgs =
            new DependencyPropertyChangedEventArgs<TProperty>(e.Property, (TProperty)e.OldValue, (TProperty)e.NewValue);

            propertyChangedCallback?.Invoke((TView)d, eArgs);
        };

        return DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TView),
            new PropertyMetadata(default(TProperty), callback)
            );
    }

}

public delegate void PropertyChangedCallback<TDependencyObject, TProperty>(TDependencyObject d, DependencyPropertyChangedEventArgs<TProperty> e)
    where TDependencyObject : DependencyObject;

public readonly struct DependencyPropertyChangedEventArgs<T>
{
    public DependencyPropertyChangedEventArgs(DependencyProperty property, T oldValue, T newValue)
    {
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public readonly DependencyProperty Property;
    public readonly T OldValue;
    public readonly T NewValue;
}
