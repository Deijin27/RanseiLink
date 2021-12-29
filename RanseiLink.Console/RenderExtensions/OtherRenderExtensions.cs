using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Core.Text;

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

    public static void Render(this IConsole console,  Message msg)
    {
        console.WriteTitle("Message");
        console.WriteProperty("Group ID", msg.GroupId);
        console.WriteProperty("Element ID", msg.ElementId);
        console.WriteProperty("Context", msg.Context);
        console.WriteProperty("Box Config", msg.BoxConfig);
        console.WriteProperty("Text", msg.Text);
    }
}
