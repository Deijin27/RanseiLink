using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("pokemon", Description = "Get data on a given pokemon.")]
    public class PokemonCommand : BaseCommand
    {
        public PokemonCommand(IServiceContainer container) : base(container) { }
        public PokemonCommand() : base() { }

        [CommandParameter(0, Description = "Pokemon ID.", Name = "id")]
        public PokemonId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Pokemon.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
