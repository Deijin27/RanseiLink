﻿using RanseiLink.PluginModule.Api;

namespace MassActionPlugin;

[Plugin("Mass Action", "Deijin", "1.0")]
public class MassActionPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var massAction = new MassAction();
        massAction.Run(context);
    }
}