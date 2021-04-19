using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RanseiConsole.Dev
{
    [Command("dev getbits pokemon", Description = "Get data on a given move.")]
    public class DevGetBitsPokemonCommand : ICommand
    {
        public IDataService<PokemonId, Pokemon> Service = new DataService();

        [CommandParameter(0, Description = "Pokemon ID.", Name = "id")]
        public PokemonId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var pokemon = Service.Retrieve(Id);
            string bits = Testing.GetBits(pokemon);
            console.Output.WriteLine(bits);

            return default;
        }
    }
}
