using System;
using System.Collections.Generic;

namespace RanseiLink.Core.Services
{
    public static class LinkCalculator
    {
        public static Dictionary<double, ushort> KnownLinkToExp => _knownLinkToExp;

        private static Dictionary<double, ushort> _knownLinkToExp;
        private static Dictionary<ushort, double> _knownExpToLink;

        static LinkCalculator()
        {
            // We keep a few known values for the people
            // who want the values to be exact
            // since the formula is only an approximation 
            _knownLinkToExp = new Dictionary<double, ushort>()
            {
                { 60,  9100 },
                { 70, 11874 },
                { 75, 13470 },
                { 80, 15217 },
                { 85, 17130 },
                { 95, 21480 },
            };

            _knownExpToLink = new Dictionary<ushort, double>();
            foreach (var kvp in _knownLinkToExp)
            {
                _knownExpToLink[kvp.Value] = kvp.Key;
            }
        }

        /// <summary>
        /// Calculate the link at a given exp. This is an estimate based on a curve fit,
        /// we don't have the exact formula yet
        /// </summary>
        public static double CalculateLink(ushort exp)
        {
            if (_knownExpToLink.TryGetValue(exp, out var link))
            {
                return link;
            }
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
            if (_knownLinkToExp.TryGetValue(link, out var exp))
            {
                return exp;
            }
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
