using CliFx.Infrastructure;
using Core.Services;

namespace RanseiConsole
{
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
}
