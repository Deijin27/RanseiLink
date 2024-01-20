#nullable enable
namespace RanseiLink.GuiCore.Services;

public interface IClipboardService
{
    Task<string?> GetText();
    Task<bool> SetText(string text);
}
