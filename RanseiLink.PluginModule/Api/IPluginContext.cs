using RanseiLink.Core.Services;

namespace RanseiLink.PluginModule.Api;

public interface IPluginContext
{
    IServiceGetter Services { get; }
}

public record PluginContext(IServiceGetter Services) : IPluginContext;