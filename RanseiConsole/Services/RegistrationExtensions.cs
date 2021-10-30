using Core.Services;
using RanseiConsole.Services.Concrete;

namespace RanseiConsole.Services
{
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
}
