using Core.Services;
using RanseiWpf.Services.Concrete;

namespace RanseiWpf.Services
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
