
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "2.1")]
public class RandomizerPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var randomizer = new Randomizer();

        var optionService = context.Services.Get<IPluginService>();
        var options = new RandomizationOptionForm();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.Services.Get<IDialogService>();

        dialogService.ProgressDialog(progress =>
        {
            randomizer.Run(context, options, progress);
        });
    }
}
