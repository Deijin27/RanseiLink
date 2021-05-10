using System;
using System.Windows;
using System.Linq.Expressions;

namespace RanseiWpf
{
    public static class UserControlUtil
    {
        public static DependencyProperty RegisterUIDependencyProperty<TView, TProperty>(
            Expression<Func<TView, TProperty>> property,
            TProperty defaultValue = default)
            where TView : DependencyObject
        {
            var expression = (MemberExpression)property.Body;
            var propertyName = expression.Member.Name;

            return DependencyProperty.Register(
                propertyName,
                typeof(TProperty),
                typeof(TView),
                new UIPropertyMetadata(defaultValue)
                );
        }

        public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
            Expression<Func<TView, TProperty>> property,
            TProperty defaultValue = default, PropertyChangedCallback propertyChangedCallback = null)
            where TView : DependencyObject
        {
            var expression = (MemberExpression)property.Body;
            var propertyName = expression.Member.Name;

            PropertyMetadata metadata;
            if (propertyChangedCallback == null)
            {
                metadata = new PropertyMetadata(defaultValue);
            }
            else
            {
                metadata = new PropertyMetadata(defaultValue, propertyChangedCallback);
            }

            return DependencyProperty.Register(
                propertyName,
                typeof(TProperty),
                typeof(TView),
                metadata
                );
        }
    }
}
