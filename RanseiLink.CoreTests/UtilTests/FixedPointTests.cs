using FluentAssertions;
using RanseiLink.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RanseiLink.CoreTests.UtilTests;

public class FixedPointTests
{
    [Theory]
    [InlineData(0x80000, 1, 19, 12, 128f)]
    [InlineData(0x20, 1, 19, 12, 0.0078125f)]
    [InlineData(0xF494, 1, 3, 12, -0.7138672f)]
    [InlineData(0x380, 1, 3, 12, 0.21875f)]
    [InlineData(0xD4FA, 1, 3, 12, -2.6889648f)]
    [InlineData(0x351, 1, 3, 6, -2.734375f)]
    [InlineData(0x19, 1, 3, 6, 0.390625f)]
    public void FixCorrectly(int input, int signBits, int intBits, int fracBits, float expectedOutput)
    {
        FixedPoint.Fix(input, signBits, intBits, fracBits).Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(0x80000, 1, 19, 12, 128f)]
    [InlineData(0x20, 1, 19, 12, 0.0078125f)]
    [InlineData(0xF494, 1, 3, 12, -0.7138672f)]
    [InlineData(0x380, 1, 3, 12, 0.21875f)]
    [InlineData(0xD4FA, 1, 3, 12, -2.6889648f)]
    [InlineData(0x351, 1, 3, 6, -2.734375f)]
    [InlineData(0x19, 1, 3, 6, 0.390625f)]
    public void InverseFixCorrectly(int expectedOutput, int signBits, int intBits, int fracBits, float input)
    {
        FixedPoint.InverseFix(input, signBits, intBits, fracBits).Should().Be(expectedOutput);
    }
}
