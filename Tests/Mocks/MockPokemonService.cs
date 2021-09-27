using Core.Enums;
using Core.Models.Interfaces;
using Core.Services.ModelServices;
using System;
using System.Collections.Generic;

namespace Tests.Mocks
{
    internal class MockPokemonService : IPokemonService
    {
        public IDisposablePokemonService Disposable()
        {
            throw new NotImplementedException();
        }

        public IDictionary<PokemonId, IPokemon> RetrieveReturn { get; set; } = new Dictionary<PokemonId, IPokemon>();
        public Queue<PokemonId> RetrieveCalls = new Queue<PokemonId>();
        public IPokemon Retrieve(PokemonId id)
        {
            RetrieveCalls.Enqueue(id);
            return RetrieveReturn[id];
        }

        public Queue<(PokemonId id, IPokemon model)> SaveCalls = new Queue<(PokemonId id, IPokemon model)>();
        public void Save(PokemonId id, IPokemon model)
        {
            SaveCalls.Enqueue((id, model));
        }
    }
}
