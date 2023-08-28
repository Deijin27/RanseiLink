
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System.Threading.Tasks;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "3.1")]
public class RandomizerPlugin : IPlugin
{
    public async Task Run(IPluginContext context)
    {
        var randomizer = new Randomizer();

        var optionService = context.Services.Get<IPluginService>();
        var options = new RandomizationOptionForm();
        if (!await optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.Services.Get<IDialogService>();

        await dialogService.ProgressDialog(progress =>
        {
            randomizer.Run(context, options, progress);
        });
    }
}
