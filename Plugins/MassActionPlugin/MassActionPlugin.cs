using RanseiLink.PluginModule.Api;

namespace MassActionPlugin;

[Plugin("Mass Action", "Deijin", "2.1")]
public class MassActionPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var massAction = new MassAction();
        massAction.Run(context);
    }
}
