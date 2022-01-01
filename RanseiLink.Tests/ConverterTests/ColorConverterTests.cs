using RanseiLink.Core.Types;
using RanseiLink.ValueConverters;
using System.Windows.Data;
using System.Windows.Media;
using Xunit;

namespace RanseiLink.Tests.ConverterTests;

public class ColorConverterTests
{
    private readonly IValueConverter _converter;

    public ColorConverterTests()
    {
        _converter = new Rgb555ToColorConverter();
    }

    [Fact]
    public void IdenticalThroughConversionCycle()
    {
        for (uint i = 0; i < 31; i++)
        {
            Rgb555 initial = new(i, 0, 0);
            Color midpoint = (Color)_converter.Convert(initial, typeof(Color), null, null);
            Rgb555 final = (Rgb555)_converter.ConvertBack(midpoint, typeof(Rgb555), null, null);
            Assert.Equal(initial, final);
        }
    }

}
