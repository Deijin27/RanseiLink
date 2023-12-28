#nullable enable
namespace RanseiLink.GuiCore.Services;

public interface IPathToImageConverter
{
    object? TryConvert(string path);
}
