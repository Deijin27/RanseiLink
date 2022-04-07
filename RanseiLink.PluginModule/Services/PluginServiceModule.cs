using Autofac;
using RanseiLink.PluginModule.Services.Concrete;

namespace RanseiLink.PluginModule.Services;

public class PluginServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PluginFormLoader>().As<IPluginFormLoader>().SingleInstance();
        builder.RegisterType<PluginLoader>().As<IPluginLoader>().SingleInstance();
    }
}