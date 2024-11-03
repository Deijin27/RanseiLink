using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

namespace RanseiLink.Core.Resources;

public static class ResourceUtil
{
    public static Stream GetResourceStream(string fileName)
    {
        return Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("RanseiLink.Core.Resources." + fileName) 
            ?? throw new Exception($"Could not find resource file '{fileName}'");
    }

    public static Image<Rgba32> GetResourceImage(string fileName)
    {
        using var stream = GetResourceStream(fileName);
        return Image.Load<Rgba32>(stream);
    }
}
