using RanseiLink.Core.Services;

namespace RanseiLink.PluginModule.Api.Concrete;

internal record PluginContext(IServiceContainer ServiceContainer, ModInfo ActiveMod) : IPluginContext;
