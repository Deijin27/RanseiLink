using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.GuiCore.Services.Concrete;
using RanseiLink.PluginModule.Services;

namespace RanseiLink.GuiCore;

public class GuiCoreServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IExternalService, ExternalService>(Reuse.Singleton);
        builder.Register<IFallbackSpriteManager, FallbackSpriteManager>(Reuse.Singleton);

        builder.RegisterDelegate(context =>
            new ModListItemViewModelFactory((mod) =>
                new ModListItemViewModel(
                    mod,
                    context.Resolve<IModManager>(),
                    context.Resolve<IModPatchingService>(),
                    context.Resolve<IAsyncDialogService>(),
                    context.Resolve<ISettingService>(),
                    context.Resolve<IPluginLoader>(),
                    context.Resolve<IModServiceGetterFactory>(),
                    context.Resolve<IFileDropHandlerFactory>(),
                    context.Resolve<IFolderDropHandler>(),
                    context.Resolve<IPathToImageConverter>()
            )), Reuse.Singleton);
    }
}

public class GuiCoreModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<ICachedSpriteProvider, CachedSpriteProvider>(Reuse.Singleton);
        builder.Register<IIdToNameService, IdToNameService>(Reuse.Singleton);
        builder.Register<ISpriteManager, SpriteManager>(Reuse.Singleton);
        builder.Register<IMapManager, MapManager>(Reuse.Singleton);
        builder.Register<IAnimGuiManager, AnimGuiManager>(Reuse.Singleton);

        builder.Register<MsgGridViewModel>();

        builder.Register<SpriteItemViewModel>();
    }
}