using System;
using System.Diagnostics;

namespace RanseiLink.Core.Graphics
{
    public static class FixedPoint
    {
        /// <summary>
        /// 1, 19, 12
        /// </summary>
        public static float Fix_1_19_12(int value)
        {
            return Fix(value, 1, 19, 12);
        }

        /// <summary>
        /// 1, 3, 12
        /// </summary>
        public static float Fix_1_3_12(int value)
        {
            return Fix(value, 1, 3, 12);
        }

        public static float Fix(int value, int signBits, int intBits, int fracBits)
        {
            Debug.Assert(signBits <= 1);
            Debug.Assert(intBits + fracBits >= 0);
            Debug.Assert(signBits + intBits + fracBits <= 32);

            double result;

            if (signBits == 0)
            {
                result = value;
            }
            else
            {
                var signMask = 1 << (intBits + fracBits);
                if ((value & signMask) != 0)
                {
                    result = value | ~(signMask - 1);
                }
                else
                {
                    result = value;
                }
            }

            return (float)(result * Math.Pow(0.5, fracBits));
        }
    }
}
