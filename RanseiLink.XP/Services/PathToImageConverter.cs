using RanseiLink.XP.ValueConverters;

namespace RanseiLink.XP.Services;
internal class PathToImageConverter : IPathToImageConverter
{
    public object TryConvert(string path)
    {
        return PathToImageSourceConverter.TryConvert(path);
    }
}
