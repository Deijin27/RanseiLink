#nullable enable
using DryIoc;

namespace RanseiLink.Core;
public interface IModule
{
    // Here we are using registration role of DryIoc Container for the builder
    void Load(IRegistrator builder);
}

public interface IModModule
{
    // Here we are using registration role of DryIoc Container for the builder
    void Load(IRegistrator builder);
}

public static class RegistratorExtensions
{
    public static void RegisterModule(this IRegistrator registrator, IModule module) => module.Load(registrator);
}
