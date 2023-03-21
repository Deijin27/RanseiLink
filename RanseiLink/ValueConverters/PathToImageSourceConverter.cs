#nullable enable
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RanseiLink.ValueConverters;

public class PathToImageSourceConverter : ValueConverter<string, ImageSource?>
{
    public static ImageSource? TryConvert(string file)
    {
        if (!File.Exists(file))
        {
            return null;
        }
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
        if (fs.Length == 0)
        {
            return null;
        }
        try
        {
            return BitmapFrame.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        catch
        {
            return null;
        }
    }
    protected override ImageSource? Convert(string value)
    {
        return TryConvert(value);
    }

    protected override string ConvertBack(ImageSource? value)
    {
        throw new NotImplementedException();
    }
}
