using System;

namespace RanseiLink.PluginModule.Api;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginAttribute : Attribute
{
    public PluginAttribute(string displayName, string author = "", string version = "")
    {
        DisplayName = displayName;
        Author = author;
        Version = version;
    }

    public string DisplayName { get; }
    public string Author { get; }
    public string Version { get; }
}
