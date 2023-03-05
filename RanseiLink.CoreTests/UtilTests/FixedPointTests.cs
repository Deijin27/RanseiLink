using RanseiLink.Core.Util;

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
    [InlineData(0, 1, 19, 12, -0.000009f)] // this test is important. The game flips out at zero with a negative flag, so that flag should not be incuded if the value gets rounded to zero
    [InlineData(0, 1, 19, 12, -0f)]
    [InlineData(0x1FF, 1, 0, 9, 1f)] // make sure vertex normals work correctly if they are 1
    public void InverseFixCorrectly(int expectedOutput, int signBits, int intBits, int fracBits, float input)
    {
        FixedPoint.InverseFix(input, signBits, intBits, fracBits).Should().Be(expectedOutput);
    }
}
