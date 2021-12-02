using RanseiLink.Core.Services;

namespace RanseiLink.PluginModule.Api;

public interface IPluginContext
{
    public IServiceContainer ServiceContainer { get; }
    public ModInfo ActiveMod { get; }
}

public record PluginContext(IServiceContainer ServiceContainer, ModInfo ActiveMod) : IPluginContext;