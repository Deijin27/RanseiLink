using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RanseiLink.PluginModule.Services;

public record PluginInfo(IPlugin Plugin, string Name, string Author, string Version);

public interface IPluginLoader
{
    IReadOnlyCollection<TPlugin> GenericLoadPlugins<TPlugin, TPluginAttribute>(DirectoryInfo pluginDirectory, out PluginLoadFailureInfo failures) where TPluginAttribute : Attribute;
    List<Assembly> LoadAssemblies(DirectoryInfo directory, out List<AssemblyLoadFailureInfo> failedToLoad);
    IReadOnlyCollection<PluginInfo> LoadPlugins(out PluginLoadFailureInfo failures);
}
