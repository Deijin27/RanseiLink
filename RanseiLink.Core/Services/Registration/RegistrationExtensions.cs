using RanseiLink.Core.Nds;
using RanseiLink.Core.Services.Concrete;
using System;
using System.IO;

namespace RanseiLink.Core.Services.Registration
{
    public static class RegistrationExtensions
    {
        public static void RegisterCoreServices(this IServiceContainer container)
        {
            string rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RanseiLink");
            Directory.CreateDirectory(rootFolder);

            NdsFactory ndsFactory = i => new Nds.Nds(i);
            container.RegisterSingleton(ndsFactory);

            container.RegisterSingleton<IModService>(new ModService(rootFolder, ndsFactory));

            container.RegisterSingleton<ISettingsService>(new SettingsService(rootFolder));

            container.RegisterSingleton<DataServiceFactory>(m => new DataService(m));
        }
    }
}
