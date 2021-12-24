
using RanseiLink.PluginModule.Api;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "1.6")]
public class RandomizerPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var randomizer = new Randomizer();
        randomizer.Run(context);
    }
}
