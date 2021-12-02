namespace RanseiLink.PluginModule.Api;

/// <summary>
/// A plugin that runs in the context of a particualar mod
/// </summary>
public interface IPlugin
{
    void Run(IPluginContext context);
}


