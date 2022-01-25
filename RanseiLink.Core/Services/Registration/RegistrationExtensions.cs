using RanseiLink.Core.Nds;
using RanseiLink.Core.Services.Concrete;
using System;
using System.IO;

namespace RanseiLink.Core.Services.Registration;

public static class RegistrationExtensions
{
    public static void RegisterCoreServices(this IServiceContainer container)
    {
        string rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RanseiLink");
        Directory.CreateDirectory(rootFolder);

        container.RegisterSingleton<NdsFactory>(i => new Nds.Nds(i));

        container.RegisterLazySingleton<IMsgService>(() => new MsgService());

        container.RegisterLazySingleton<IModService>(() => new ModService(rootFolder, container.Resolve<NdsFactory>(), container.Resolve<IMsgService>()));

        container.RegisterLazySingleton<ISettingsService>(() => new SettingsService(rootFolder));

        container.RegisterLazySingleton<ISpriteService>(() => new SpriteService());

        container.RegisterSingleton<DataServiceFactory>(m => new DataService(m, container));
    }
}
