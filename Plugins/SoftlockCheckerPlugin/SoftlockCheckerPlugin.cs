using RanseiLink.PluginModule.Api;

namespace SoftlockCheckerPlugin;

[Plugin("Softlock Checker", "Deijin", "1.0")]
public class SoftlockCheckerPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var checker = new SoftlockChecker();
        checker.Run(context);
    }
}