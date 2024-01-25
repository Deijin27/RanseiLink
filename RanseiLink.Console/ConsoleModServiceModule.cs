using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;

namespace RanseiLink.Console;

public class ConsoleModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IModServiceContainer, ModServiceContainer>(Reuse.Singleton);
        builder.Register<ILuaService, LuaService>(Reuse.Singleton);
    }
}
