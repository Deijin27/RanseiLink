using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services.Concrete;

namespace RanseiLink.Services;

public static class RegistrationExtensions
{
    public static void RegisterWpfServices(this IServiceContainer container)
    {
        var settingsService = container.Resolve<ISettingsService>();
        var pluginFormLoader = container.Resolve<IPluginFormLoader>();

        container.RegisterSingleton<IDialogService>(new DialogService(settingsService));
        container.RegisterSingleton<IPluginService>(new PluginService(pluginFormLoader));
        container.RegisterSingleton<IThemeService>(new ThemeService(settingsService));
    }
}
