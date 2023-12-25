using DryIoc;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.DefaultPopulaters;
using RanseiLink.Core.Settings;
using System.Reflection;

namespace RanseiLink.Core
{
    public class CoreServiceModule : IModule
    {
        public void Load(IRegistrator builder)
        {
            RomFsFactory romFsFactory = file => new RomFs.RomFs(file);
            builder.RegisterInstance(romFsFactory);

            builder.Register<IMsgService, MsgService>(Reuse.Singleton);
            builder.Register<IFallbackDataProvider, FallbackDataProvider>(Reuse.Singleton);
            builder.Register<IModPatchingService, ModPatchingService>(Reuse.Singleton);

            string modFolder = Path.Combine(Constants.RootFolder, "Mods");
            builder.Register<IModManager, ModManager>(Reuse.Singleton,
                Made.Of(() => new ModManager(
                    modFolder,
                    Arg.Of<RomFsFactory>(),
                    Arg.Of<IMsgService>()
                )));

            string settingFolder = Path.Combine(Constants.RootFolder, "RanseiLinkSettings.xml");
            builder.Register<ISettingService, SettingService>(Reuse.Singleton,
                Made.Of(() => new SettingService(
                    settingFolder
                    )));

            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsAbstract || type.GetCustomAttribute<DefaultPopulaterAttribute>() == null)
                {
                    continue;
                }
                if (typeof(IGraphicTypeDefaultPopulater).IsAssignableFrom(type))
                {
                    builder.Register(typeof(IGraphicTypeDefaultPopulater), type, Reuse.Singleton);
                }
                else if (typeof(IMiscItemDefaultPopulater).IsAssignableFrom(type))
                {
                    builder.Register(typeof(IMiscItemDefaultPopulater), type, Reuse.Singleton);
                }
            }
        }
    }
}