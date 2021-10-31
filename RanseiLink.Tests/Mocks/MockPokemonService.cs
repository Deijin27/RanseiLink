using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services.ModelServices;
using System;
using System.Collections.Generic;

namespace RanseiLink.Tests.Mocks
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

        public IEvolutionTable RetrieveEvolutionTableReturn { get; set; }
        public int RetrieveEvolutionTableCallCount = 0;
        public IEvolutionTable RetrieveEvolutionTable()
        {
            RetrieveEvolutionTableCallCount++;
            return RetrieveEvolutionTableReturn;
        }

        public Queue<IEvolutionTable> SaveEvolutionTableCalls = new Queue<IEvolutionTable>();
        public void SaveEvolutionTable(IEvolutionTable model)
        {
            SaveEvolutionTableCalls.Enqueue(model);
        }
    }
}
