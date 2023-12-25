using DryIoc;
using RanseiLink.Core;
using RanseiLink.GuiCore.Services.Concrete;

namespace RanseiLink.GuiCore;

public class GuiCoreServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IExternalService, ExternalService>(Reuse.Singleton);
        builder.Register<IFallbackSpriteManager, FallbackSpriteManager>(Reuse.Singleton);
    }
}

public class GuiCoreModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IIdToNameService, IdToNameService>(Reuse.Singleton);
        builder.Register<ISpriteManager, SpriteManager>(Reuse.Singleton);
        builder.Register<IMapManager, MapManager>(Reuse.Singleton);
        builder.Register<IAnimGuiManager, AnimGuiManager>(Reuse.Singleton);
    }
}