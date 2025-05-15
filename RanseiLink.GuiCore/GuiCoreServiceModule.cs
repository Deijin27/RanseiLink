using DryIoc;
using RanseiLink.Core;
using RanseiLink.GuiCore.Services.Concrete;

namespace RanseiLink.GuiCore;

public class GuiCoreServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IUpdateService, UpdateService>(Reuse.Singleton);
        builder.Register<IExternalService, ExternalService>(Reuse.Singleton);
        builder.Register<IFallbackSpriteManager, FallbackSpriteManager>(Reuse.Singleton);
        builder.Register<IMapMiniPreviewImageGenerator, MapMiniPreviewImageGenerator>(Reuse.Singleton);

        builder.Register<MainWindowViewModel>(Reuse.Singleton);
        builder.Register<IMainEditorViewModel, MainEditorViewModel>(Reuse.Singleton);
        builder.Register<IModSelectionViewModel, ModSelectionViewModel>(Reuse.Singleton);
        builder.Register<ModListItemViewModel>(Reuse.Transient);
        builder.RegisterDelegate(context => 
            new ModListItemViewModelFactory((mod, getKnownTags) => context.Resolve<ModListItemViewModel>().Init(mod, getKnownTags))
            , Reuse.Singleton);
        builder.Register<IJumpService, JumpService>(Reuse.Singleton);

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
        builder.Register<INicknameService, NicknameService>(Reuse.Singleton);

        builder.Register<MsgGridViewModel>();

        builder.Register<SpriteItemViewModel>();
    }
}