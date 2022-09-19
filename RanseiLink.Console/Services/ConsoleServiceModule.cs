using CliFx;
using CliFx.Attributes;
using DryIoc;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using System.Reflection;

namespace RanseiLink.Console.Services;

public class ConsoleServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<ICurrentModService, CurrentModService>(Reuse.Singleton);
        builder.Register<PathConverter>(Reuse.Singleton);

        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (typeof(ICommand).IsAssignableFrom(type)
                && type.GetCustomAttribute<CommandAttribute>() != null
                && !type.IsAbstract)
            {
                builder.Register(type);
            }
        }

        ContainerProvider.ModServiceGetterFactory.AddModule(new ConsoleModServiceModule());
    }
}

public class ConsoleModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IModServiceContainer, ModServiceContainer>(Reuse.Singleton);
        builder.Register<ILuaService, LuaService>(Reuse.Singleton);
    }
}
