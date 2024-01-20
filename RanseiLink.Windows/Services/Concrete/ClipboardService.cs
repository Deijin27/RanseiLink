#nullable enable
namespace RanseiLink.Windows.Services.Concrete;

public class ClipboardService : IClipboardService
{
    public Task<string?> GetText()
    {
        try
        {
            return Task.FromResult<string?>(System.Windows.Clipboard.GetText());
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
        
    }

    public Task<bool> SetText(string text)
    {
        try
        {
            System.Windows.Clipboard.SetText(text);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
