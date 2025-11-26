using RanseiLink.PluginModule.Api;
using System.Threading.Tasks;

namespace SoftlockCheckerPlugin;

[Plugin("Softlock Checker", "Deijin", "2.6")]
public class SoftlockCheckerPlugin : IPlugin
{
    public Task Run(IPluginContext context)
    {
        var checker = new SoftlockChecker();
        return checker.Run(context);
    }
}