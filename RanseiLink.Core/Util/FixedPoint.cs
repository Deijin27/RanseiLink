#nullable enable
using System;
using System.Diagnostics;

namespace RanseiLink.Core.Util;

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

    /// <summary>
    /// 1, 19, 12
    /// </summary>
    public static int InverseFix_1_19_12(float value)
    {
        return InverseFix(value, 1, 19, 12);
    }

    /// <summary>
    /// 1, 3, 12
    /// </summary>
    public static ushort InverseFix_1_3_12(float value)
    {
        return (ushort)InverseFix(value, 1, 3, 12);
    }

    public static float Fix(int value, int signBits, int intBits, int fracBits)
    {
        Debug.Assert(signBits == 0 || signBits == 1);
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

    public static int InverseFix(float value, int signBits, int intBits, int fracBits)
    {
        Debug.Assert(signBits == 0 || signBits == 1);
        Debug.Assert(intBits + fracBits >= 0);
        Debug.Assert(signBits + intBits + fracBits <= 32);

        // coerce value into correct range (i know the implementation is slow, but it doesn't matter
        var min = MinValue(signBits, intBits, fracBits);
        if (value < min)
        {
            value = min;
        }
        else
        {
            var max = MaxValue(signBits, intBits, fracBits);
            if (value > max)
            {
                value = max;
            }
        }

        // calculate result
        double dbl = value * Math.Pow(2, fracBits);

        int result = (int)Math.Round(dbl);
        if (signBits != 0 && value < 0 && result != 0)
        {
            int signMask = 1 << (intBits + fracBits);
            result &= signMask - 1;
            result |= signMask;
        }
        return result;
    }

    public static float MaxValue(int signBits, int intBits, int fracBits)
    {
        int signMask = 1 << (intBits + fracBits);
        return FixedPoint.Fix(signMask - 1, signBits, intBits, fracBits);
    }

    public static float MinValue(int signBits, int intBits, int fracBits)
    {
        if (signBits != 0)
        {
            return FixedPoint.Fix(1 << (intBits + fracBits), signBits, intBits, fracBits);
        }
        else
        {
            return 0;
        }
        
    }
}
