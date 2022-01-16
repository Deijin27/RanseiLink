using System;

namespace RanseiLink.Core.Services;

public static class LinkCalculator
{
    /// <summary>
    /// Calculate the link at a given exp. This is an estimate based on a curve fit,
    /// we don't have the exact formula yet
    /// </summary>
    public static double CalculateLink(ushort exp)
    {
        return -0.123 + 0.0106 * exp - 5.92E-7 * Math.Pow(exp, 2) + 1.64E-11 * Math.Pow(exp, 3);
    }

    /// <summary>
    /// Calculate the exp at a given link. This is an estimate based on a curve fit,
    /// we don't have the exact formula yet
    /// </summary>
    public static ushort CalculateExp(double link)
    {
        return (ushort)Math.Round(
            0.219 +  99.9 * link + 0.0644 * Math.Pow(link, 2) +  0.0133 * Math.Pow(link, 3)
            );
    }
}
