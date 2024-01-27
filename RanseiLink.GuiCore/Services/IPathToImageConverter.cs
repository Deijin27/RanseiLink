#nullable enable
namespace RanseiLink.GuiCore.Services;

public interface IPathToImageConverter
{
    object? TryConvert(string path);
    object? TryCrop(object? image, int x, int y, int width, int height);
}
