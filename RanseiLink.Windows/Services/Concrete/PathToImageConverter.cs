#nullable enable
using RanseiLink.Windows.ValueConverters;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RanseiLink.Windows.Services.Concrete;

public class PathToImageConverter : IPathToImageConverter
{
    public object? TryConvert(string path)
    {
        return PathToImageSourceConverter.TryConvert(path);
    }

    public object? TryCrop(object? image, int x, int y, int width, int height)
    {
        if (image is BitmapSource bitmapSource)
        {
            if ((width + x) > bitmapSource.PixelWidth || (height + y) > bitmapSource.PixelHeight)
            {
                return null;
            }
            return new CroppedBitmap(bitmapSource, new System.Windows.Int32Rect(x, y, width, height));
        }
        else
        {
            return null;
        }
    }
}
