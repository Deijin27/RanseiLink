using RanseiLink.Core.Graphics;
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
        for (int i = 0; i < 31; i++)
        {
            Rgb15 initial = new(i, 0, 0);
            Color midpoint = (Color)_converter.Convert(initial, typeof(Color), null, null);
            Rgb15 final = (Rgb15)_converter.ConvertBack(midpoint, typeof(Rgb15), null, null);
            Assert.Equal(initial, final);
        }
    }

}
