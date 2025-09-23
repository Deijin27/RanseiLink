using RanseiLink.Core.Util;

namespace RanseiLink.CoreTests.UtilTests;

public class InvariantNumberTests
{
    [Fact]
    public void ParseFloat()
    {
        var result = InvariantNumber.ParseFloat("1.3");
        result.Should().Be(1.3f);
    }

    [Fact]
    public void TryParseFloat()
    {
        var result = InvariantNumber.TryParseFloat("1.3", out var num);
        result.Should().Be(true);
        num.Should().Be(1.3f);
    }

    [Fact]
    public void SeralizeFloat()
    {
        var result = InvariantNumber.FloatToString(1.3f);
        result.Should().Be("1.3");
    }

    [Fact]
    public void ParseDouble()
    {
        var result = InvariantNumber.ParseDouble("1.3");
        result.Should().Be(1.3);
    }

    [Fact]
    public void TryParseDouble()
    {
        var result = InvariantNumber.TryParseDouble("1.3", out var num);
        result.Should().Be(true);
        num.Should().Be(1.3);
    }

    [Fact]
    public void SeralizeDouble()
    {
        var result = InvariantNumber.DoubleToString(1.3);
        result.Should().Be("1.3");
    }

    [Fact]
    public void ParseFloatComma()
    {
        var result = InvariantNumber.ParseFloat("1,3");
        result.Should().Be(1.3f);
    }

    [Fact]
    public void ParseDoubleComma()
    {
        var result = InvariantNumber.ParseDouble("1,3");
        result.Should().Be(1.3);
    }
}
