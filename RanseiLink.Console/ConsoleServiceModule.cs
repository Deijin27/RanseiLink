using CliFx;
using CliFx.Attributes;
using DryIoc;
using RanseiLink.Console.Services;
using RanseiLink.Core;
using System.Reflection;

namespace RanseiLink.Console;

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
    }
}
