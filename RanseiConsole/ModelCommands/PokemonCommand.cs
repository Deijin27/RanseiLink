using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("pokemon", Description = "Get data on a given pokemon.")]
    public class PokemonCommand : ICommand
    {
        [CommandParameter(0, Description = "Pokemon ID.", Name = "id")]
        public PokemonId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.Pokemon.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
