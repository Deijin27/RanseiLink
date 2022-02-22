using RanseiLink.Core.Nds;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Settings;
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

        container.RegisterLazySingleton<IFallbackSpriteProvider>(() => new FallbackSpriteProvider(rootFolder, container.Resolve<NdsFactory>()));

        container.RegisterLazySingleton<IModService>(() => new ModService(rootFolder, container.Resolve<NdsFactory>(), 
            container.Resolve<IMsgService>(), container.Resolve<IFallbackSpriteProvider>()));

        container.RegisterLazySingleton<ISettingService>(() => new SettingService(Path.Combine(rootFolder, "RanseiLinkSettings.xml")));

        container.RegisterSingleton<DataServiceFactory>(m => new DataService(m, container));
    }
}
