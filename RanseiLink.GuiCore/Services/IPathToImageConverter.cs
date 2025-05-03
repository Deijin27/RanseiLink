#nullable enable
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.Services;

public interface IPathToImageConverter
{
    object? TryConvert(string path);
    object? TryCrop(object? image, int x, int y, int width, int height);
}

public static class PathToImageExtensions
{
    public static object? TryConvert(this IPathToImageConverter converter, Image<Rgba32>? image)
    {
        if (image == null)
        {
            return null;
        }
        var temp = Path.GetTempFileName();
        try
        {
            image.SaveAsPng(temp);
            return converter.TryConvert(temp);
        }
        catch
        {
            return null;
        }
        finally
        {
            File.Delete(temp);
        }
    }
}