using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("pokemon", Description = "Get data on a given pokemon.")]
public class PokemonCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public PokemonCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Pokemon ID.", Name = "id")]
    public PokemonId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IPokemonService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
