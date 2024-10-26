using RanseiLink.GuiCore.Services;
using RanseiLink.PluginModule.Api;
using System.Threading.Tasks;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "3.2")]
public class RandomizerPlugin : IPlugin
{
    public async Task Run(IPluginContext context)
    {
        var randomizer = new Randomizer();

        var optionService = context.Services.Get<IAsyncPluginService>();
        var options = new RandomizationOptionForm();
        if (!await optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.Services.Get<IAsyncDialogService>();

        await dialogService.ProgressDialog(progress =>
        {
            randomizer.Run(context, options, progress);
        });
    }
}
