using System;

namespace RanseiLink.Core.Services
{
    public static class LinkCalculator
    {
        /// <summary>
        /// Calculate the link at a given exp. This is an estimate based on a curve fit,
        /// we don't have the exact formula yet
        /// </summary>
        public static double CalculateLink(ushort exp)
        {
            return 
                - 0.0568
                + 0.0106 * exp
                - 6.08E-7 * Math.Pow(exp, 2) 
                + 2.17E-11 * Math.Pow(exp, 3)
                - 3.13E-16 * Math.Pow(exp, 4);
        }

        /// <summary>
        /// Calculate the exp at a given link. This is an estimate based on a curve fit,
        /// we don't have the exact formula yet
        /// </summary>
        public static ushort CalculateExp(double link)
        {
            return (ushort)Math.Round(
                - 1.31
                + 100 * link
                + 0.0561 * Math.Pow(link, 2)
                + 0.0134 * Math.Pow(link, 3)
                - 3.24E-7 * Math.Pow(link, 4)
                );
        }
    }
}
