using Avalonia.Media;
using Avalonia.Media.Imaging;
using RanseiLink.XP.ValueConverters;

namespace RanseiLink.XP.Services;
internal class PathToImageConverter : IPathToImageConverter
{
    public object TryConvert(string path)
    {
        return PathToImageSourceConverter.TryConvert(path);
    }

    public object TryCrop(object image, int x, int y, int width, int height)
    {
        if (image is IImage bitmap)
        {
            return new CroppedBitmap(bitmap, new Avalonia.PixelRect(x, y, width, height));
        }
        return null;
    }
}
