using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.CoreTests;

internal static class AssertUtil
{
    public static void ShouldBeIdenticalTo(this Image<Rgba32> actualImage, Image<Rgba32> expectedImage)
    {
        actualImage.Width.Should().Be(expectedImage.Width);
        actualImage.Height.Should().Be(expectedImage.Height);

        for (int x = 0; x < actualImage.Width; x++)
        {
            for (int y = 0; y < actualImage.Height; y++)
            {
                actualImage[x, y].Should().Be(expectedImage[x, y]);
            }
        }
    }
}
