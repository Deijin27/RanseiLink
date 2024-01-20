#nullable enable
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace RanseiLink.XP.Services;

public class ClipboardService : IClipboardService
{
    private static IClipboard? GetClipboard() => TopLevel.GetTopLevel(App.MainWindow)?.Clipboard;

    public Task<string?> GetText()
    {
        var clipboard = GetClipboard();
        if (clipboard == null)
        {
            return Task.FromResult<string?>(null);
        }
        return clipboard.GetTextAsync();
    }

    public async Task<bool> SetText(string text)
    {
        var clipboard = GetClipboard();
        if (clipboard == null)
        {
            return false;
        }
        await clipboard.SetTextAsync(text);
        return true;
    }
}
