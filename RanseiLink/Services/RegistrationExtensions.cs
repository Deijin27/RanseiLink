using RanseiLink.Core.Services;
using RanseiLink.Services.Concrete;

namespace RanseiLink.Services
{
    public static class RegistrationExtensions
    {
        public static void RegisterWpfServices(this IServiceContainer container)
        {
            var settingsService = container.Resolve<ISettingsService>();

            container.RegisterSingleton<IDialogService>(new DialogService(settingsService));
        }
    }
}
