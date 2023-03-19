using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core
{

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class DefaultValuesAttribute : Attribute
    {
        public List<object> DefaultValues { get; }
        public DefaultValuesAttribute(params object[] defaultValues)
        {
            DefaultValues = defaultValues.ToList();
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetValuesExceptDefaults<T>()
        {
            if (!(typeof(T).GetCustomAttributes(typeof(DefaultValuesAttribute), false).FirstOrDefault() is DefaultValuesAttribute attr))
            {
                throw new Exception($"Attribute {nameof(DefaultValuesAttribute)} not found on {typeof(T).Name}");
            }

            return GetValues<T>().Where(i => !attr.DefaultValues.Contains(i!));
        }

        public static IEnumerable<T> GetValuesExceptDefaultsWithFallback<T>()
        {
            if (!(typeof(T).GetCustomAttributes(typeof(DefaultValuesAttribute), false).FirstOrDefault() is DefaultValuesAttribute attr))
            {
                return GetValues<T>();
            }

            return GetValues<T>().Where(i => !attr.DefaultValues.Contains(i!));
        }
    }
}