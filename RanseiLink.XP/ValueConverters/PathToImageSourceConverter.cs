using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace RanseiLink.XP.ValueConverters;
public class PathToImageSourceConverter
{
    public static IImage TryConvert(string file)
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
            return new Bitmap(fs);
        }
        catch
        {
            return null;
        }
    }
}