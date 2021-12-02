using RanseiLink.Core.Services;
using RanseiLink.Console.Services.Concrete;

namespace RanseiLink.Console.Services.Registration;

public static class RegistrationExtensions
{
    public static void RegisterConsoleServices(this IServiceContainer container)
    {
        var modService = container.Resolve<IModService>();
        var settingsService = container.Resolve<ISettingsService>();
        var dataServiceFactory = container.Resolve<DataServiceFactory>();

        container.RegisterSingleton<ICurrentModService>(new CurrentModService(settingsService, modService, dataServiceFactory));
    }
}
