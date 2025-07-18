﻿using DryIoc;
using RanseiLink.Core;
using RanseiLink.PluginModule.Services;
using RanseiLink.PluginModule.Services.Concrete;

namespace RanseiLink.PluginModule;

public class PluginServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IPluginFormLoader, PluginFormLoader>(Reuse.Singleton);
        builder.Register<IPluginLoader, PluginLoader>(Reuse.Singleton);
    }
}