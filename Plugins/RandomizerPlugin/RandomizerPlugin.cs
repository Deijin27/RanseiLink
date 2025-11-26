using RanseiLink.Core.Enums;
using RanseiLink.GuiCore.Services;
using RanseiLink.PluginModule.Api;
using System;
using System.Threading.Tasks;

namespace RandomizerPlugin;

[Plugin("Randomizer", "Deijin", "3.3")]
public class RandomizerPlugin : IPlugin
{
    public async Task Run(IPluginContext context)
    {
        var randomizer = new Randomizer();

        var optionService = context.Services.Get<IAsyncPluginService>();
        var options = new RandomizationOptionForm
        {
            Seed = GenerateSeed()
        };

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

    private static string GenerateSeed()
    {
        var random = new Random();
        var pokemon = (PokemonId)random.Next(200);
        var number = random.Next(10000);
        return $"{pokemon}{number}";
    }
}
