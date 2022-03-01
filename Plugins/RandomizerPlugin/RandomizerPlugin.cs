
using RanseiLink.PluginModule.Api;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "2.0")]
public class RandomizerPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var randomizer = new Randomizer();
        randomizer.Run(context);
    }
}
