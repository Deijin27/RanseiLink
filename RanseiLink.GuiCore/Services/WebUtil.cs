using System.Diagnostics;

namespace RanseiLink.GuiCore.Services;

public static class WebUtil
{
    public static void OpenUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = url
        };
        Process.Start(psi);
    }
}
