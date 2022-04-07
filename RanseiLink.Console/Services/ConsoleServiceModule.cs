using Autofac;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;

namespace RanseiLink.Console.Services;

public class ConsoleServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CurrentModService>().As<ICurrentModService>().SingleInstance();
        builder.RegisterType<ModServiceContainer>().As<IModServiceContainer>().SingleInstance();
        builder.RegisterType<LuaService>().As<ILuaService>().SingleInstance();
    }
}
