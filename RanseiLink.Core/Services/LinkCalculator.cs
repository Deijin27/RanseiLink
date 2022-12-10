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
                [0] = 0,
                [1] = 100,
                [2] = 200,
                [3] = 300,
                [4] = 402,
                [5] = 502,
                [6] = 604,
                [7] = 708,
                [8] = 810,
                [9] = 914,
                [10] = 1020,
                [11] = 1124,
                [12] = 1232,
                [13] = 1340,
                [14] = 1448,
                [15] = 1558,
                [16] = 1670,
                [17] = 1784,
                [18] = 1898,
                [19] = 2014,
                [20] = 2132,
                [21] = 2250,
                [22] = 2372,
                [23] = 2494,
                [24] = 2620,
                [25] = 2748,
                [26] = 2876,
                [27] = 3008,
                [28] = 3142,
                [29] = 3278,
                [30] = 3416,
                [31] = 3556,
                [32] = 3700,
                [33] = 3846,
                [34] = 3996,
                [35] = 4148,
                [36] = 4302,
                [37] = 4460,
                [38] = 4620,
                [39] = 4784,
                [40] = 4952,
                [41] = 5122,
                [42] = 5296,
                [43] = 5474,
                [44] = 5654,
                [45] = 5840,
                [46] = 6028,
                [47] = 6220,
                [48] = 6416,
                [49] = 6616,
                [50] = 6820,
                [51] = 7028,
                [52] = 7240,
                [53] = 7458,
                [54] = 7678,
                [55] = 7904,
                [56] = 8134,
                [57] = 8368,
                [58] = 8608,
                [59] = 8852,
                [60] = 9100,
                [61] = 9354,
                [62] = 9612,
                [63] = 9876,
                [64] = 10146,
                [65] = 10420,
                [66] = 10700,
                [67] = 10984,
                [68] = 11274,
                [69] = 11570,
                [70] = 11872,
                [71] = 12180,
                [72] = 12492,
                [73] = 12812,
                [74] = 13136,
                [75] = 13468,
                [76] = 13806,
                [77] = 14148,
                [78] = 14498,
                [79] = 14854,
                [80] = 15216,
                [81] = 15586,
                [82] = 15962,
                [83] = 16344,
                [84] = 16732,
                [85] = 17128,
                [86] = 17530,
                [87] = 17940,
                [88] = 18358,
                [89] = 18782,
                [90] = 19212,
                [91] = 19652,
                [92] = 20098,
                [93] = 20550,
                [94] = 21012,
                [95] = 21480,
                [96] = 21956,
                [97] = 22440,
                [98] = 22932,
                [99] = 23432,
                [100] = 23942
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
