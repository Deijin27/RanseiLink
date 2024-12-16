using System.Globalization;

namespace RanseiLink.Core.Util;
public static class StringUtil
{
    public static bool ContainsIgnoreCaseAndAccents(this string source, string value)
    {
        var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
        var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
        var index = compareInfo.IndexOf(source, value, options);
        return index != -1;
    }
}
