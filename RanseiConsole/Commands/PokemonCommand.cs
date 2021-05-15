using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("pokemon", Description = "Get data on a given pokemon.")]
    public class PokemonCommand : ICommand
    {
        [CommandParameter(0, Description = "Pokemon ID.", Name = "id")]
        public PokemonId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<PokemonId, IPokemon> service = new DataService();
            var move = service.Retrieve(Id);
            console.Render(move, Id);
            return default;
        }
    }
}
