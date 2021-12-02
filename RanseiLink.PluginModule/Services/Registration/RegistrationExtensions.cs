using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services.Concrete;

namespace RanseiLink.PluginModule.Services.Registration;

public static class RegistrationExtensions
{
    public static void RegisterPluginServices(this IServiceContainer container)
    {
        container.RegisterSingleton<IPluginFormLoader>(new PluginFormLoader());
        container.RegisterSingleton<IPluginLoader>(new PluginLoader());
    }
}
