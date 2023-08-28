using System.Threading.Tasks;

namespace RanseiLink.PluginModule.Api;

/// <summary>
/// A plugin that runs in the context of a particualar mod
/// </summary>
public interface IPlugin
{
    Task Run(IPluginContext context);
}


