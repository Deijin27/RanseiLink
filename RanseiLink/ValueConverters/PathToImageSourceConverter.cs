using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RanseiLink.ValueConverters;

public class PathToImageSourceConverter : ValueConverter<string, ImageSource>
{
    public static bool TryConvert(string file, out ImageSource image)
    {
        if (!File.Exists(file))
        {
            image = null;
            return false;
        }
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
        if (fs.Length == 0)
        {
            image = null;
            return false;
        }
        try
        {
            image = BitmapFrame.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            return true;
        }
        catch
        {
            image = null;
            return false;
        }
    }
    protected override ImageSource Convert(string value)
    {
        if (TryConvert(value, out ImageSource image))
        {
            return image;
        }
        return null;
    }

    protected override string ConvertBack(ImageSource value)
    {
        throw new NotImplementedException();
    }
}
