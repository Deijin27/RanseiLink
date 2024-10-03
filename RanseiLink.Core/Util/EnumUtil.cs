using System.ComponentModel;
using System.Reflection;

namespace RanseiLink.Core;


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

    public static IEnumerable<(T value, string name)> GetValuesWithNames<T>()
    {
        foreach (var value in GetValues<T>())
        {
            var memberName = value?.ToString();
            if (memberName == null)
            {
                continue;
            }
            var memberInfo = typeof(T).GetMember(memberName);
            if (memberInfo.Length == 0)
            {
                continue;
            }
            var attr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
            if (attr == null)
            {
                yield return (value, memberName);
            }
            else
            {
                yield return (value, attr.Description);
            }
        }
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