using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

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

    public static void FilesShouldBeIdentical(string filePath1, string filePath2)
    {
        var bytes1 = File.ReadAllBytes(filePath1);
        var bytes2 = File.ReadAllBytes(filePath2);
        bytes2.Should().Equal(bytes1);
    }

    public static void FileShouldBeEmpty(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        bytes.Should().BeEmpty();
    }
}
