using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services.Concrete;

namespace RanseiLink.PluginModule.Services.Registration;

public static class RegistrationExtensions
{
    public static void RegisterPluginServices(this IServiceContainer container)
    {
        container.RegisterLazySingleton<IPluginFormLoader>(() => new PluginFormLoader());
        container.RegisterLazySingleton<IPluginLoader>(() => new PluginLoader());
    }
}
