using RanseiLink.PluginModule.Api;
using System.Threading.Tasks;

namespace MassActionPlugin;

[Plugin("Mass Action", "Deijin", "2.1")]
public class MassActionPlugin : IPlugin
{
    public Task Run(IPluginContext context)
    {
        var massAction = new MassAction();
        return massAction.Run(context);
    }
}
