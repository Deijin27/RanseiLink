#nullable enable
using RanseiLink.Windows.ValueConverters;

namespace RanseiLink.Windows.Services.Concrete;

public class PathToImageConverter : IPathToImageConverter
{
    public object? TryConvert(string path)
    {
        return PathToImageSourceConverter.TryConvert(path);
    }
}
