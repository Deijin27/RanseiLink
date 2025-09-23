using DryIoc;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RanseiLink.Core.Util;

public static class InvariantNumber
{
    [return: NotNullIfNotNull(nameof(input))]
    private static string? Normalize(string? input)
    {
        return input?.Replace(',', '.');
    }

    public static bool TryParseFloat(string? input, out float result)
    {
        return float.TryParse(Normalize(input), CultureInfo.InvariantCulture, out result);
    }

    public static string FloatToString(float input)
    {
        return input.ToString(CultureInfo.InvariantCulture);
    }

    public static float ParseFloat(string input)
    {
        return float.Parse(Normalize(input), CultureInfo.InvariantCulture);
    }

    public static bool TryParseDouble(string? input, out double result)
    {
        return double.TryParse(Normalize(input), CultureInfo.InvariantCulture, out result);
    }

    public static string DoubleToString(double input)
    {
        return input.ToString(CultureInfo.InvariantCulture);
    }

    public static double ParseDouble(string input)
    {
        return double.Parse(Normalize(input), CultureInfo.InvariantCulture);
    }
}
