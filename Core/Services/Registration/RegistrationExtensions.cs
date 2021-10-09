using Core.Nds;
using Core.Services.Concrete;
using System;
using System.IO;

namespace Core.Services.Registration
{
    public static class RegistrationExtensions
    {
        public static void RegisterCoreServices(this IServiceContainer container)
        {
            string rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RanseiLink");
            Directory.CreateDirectory(rootFolder);

            INdsFactory ndsFactory = new NdsFactory();
            container.RegisterSingleton(ndsFactory);

            container.RegisterSingleton<IModService>(new ModService(rootFolder, ndsFactory));

            container.RegisterSingleton<ISettingsService>(new SettingsService(rootFolder));

            container.RegisterSingleton<IDataServiceFactory>(new DataServiceFactory());
        }
    }
}
