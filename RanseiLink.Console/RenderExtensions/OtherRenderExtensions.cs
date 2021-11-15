using CliFx.Infrastructure;
using RanseiLink.Core.Services;

namespace RanseiLink.Console;

public static partial class RenderExtensions
{
    public static void Render(this IConsole console, ModInfo modInfo, string title = "Mod Info")
    {
        console.WriteTitle(title);
        console.WriteProperty("Name", modInfo.Name);
        console.WriteProperty("Version", modInfo.Version);
        console.WriteProperty("Author", modInfo.Author);
    }
}
