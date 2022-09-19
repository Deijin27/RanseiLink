using DryIoc;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services.Concrete;

namespace RanseiLink.PluginModule.Services;

public class PluginServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IPluginFormLoader, PluginFormLoader>();
        builder.Register<IPluginLoader, PluginLoader>();
    }
}