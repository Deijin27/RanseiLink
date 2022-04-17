using Autofac;
using CliFx;
using CliFx.Attributes;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using System.Reflection;

namespace RanseiLink.Console.Services;

public class ConsoleServiceModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CurrentModService>().As<ICurrentModService>().SingleInstance();
        builder.RegisterType<PathConverter>().AsSelf().SingleInstance();

        foreach (var type in ThisAssembly.GetTypes())
        {
            if (typeof(ICommand).IsAssignableFrom(type) && type.GetCustomAttribute<CommandAttribute>() != null)
            {
                builder.RegisterType(type).AsSelf();
            }
        }

        ContainerProvider.ModServiceGetterFactory.AddModule(new ConsoleModServiceModule());
    }
}

public class ConsoleModServiceModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ModServiceContainer>().As<IModServiceContainer>().SingleInstance();
        builder.RegisterType<LuaService>().As<ILuaService>().SingleInstance();
    }
}
