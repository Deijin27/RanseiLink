using RanseiLink.Core.Services;
using RanseiLink.Console.Services.Concrete;

namespace RanseiLink.Console.Services.Registration;

public static class RegistrationExtensions
{
    public static void RegisterConsoleServices(this IServiceContainer container)
    {
        container.RegisterLazySingleton<ICurrentModService>(() => new CurrentModService(container));
    }
}
