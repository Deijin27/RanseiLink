using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands;

[Command("evolutiontable", Description = "Get evolution table data.")]
public class EvolutionTableCommand : BaseCommand
{
    public EvolutionTableCommand(IServiceContainer container) : base(container) { }
    public EvolutionTableCommand() : base() { }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IModServiceContainer dataService))
        {
            return default;
        }

        var model = dataService.Pokemon.RetrieveEvolutionTable();

        console.Render(model);

        return default;
    }
}
