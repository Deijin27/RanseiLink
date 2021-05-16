using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using System;
using System.Collections.Generic;

namespace Tests.Mocks
{
    public class MockDataService : IDataService
    {
        public Dictionary<PokemonId, IPokemon> PokemonDict = new Dictionary<PokemonId, IPokemon>();
        public Dictionary<MoveId, IMove> MoveDict = new Dictionary<MoveId, IMove>();
        public Dictionary<AbilityId, IAbility> AbilityDict = new Dictionary<AbilityId, IAbility>();
        public Dictionary<SaihaiId, ISaihai> SaihaiDict = new Dictionary<SaihaiId, ISaihai>();
        public Dictionary<GimmickId, IGimmick> GimmickDict = new Dictionary<GimmickId, IGimmick>();
        public Dictionary<BuildingId, IBuilding> BuildingDict = new Dictionary<BuildingId, IBuilding>();


        public Dictionary<PokemonId, IPokemon> AllPokemon()
        {
            var dict = new Dictionary<PokemonId, IPokemon>();
            foreach (var (key, value) in PokemonDict)
            {
                dict[key] = value.Clone();
            }
            return dict;
        }

        public void CommitToRom(string path)
        {
            throw new NotImplementedException();
        }

        public void LoadRom(string path)
        {
            throw new NotImplementedException();
        }

        public IPokemon Retrieve(PokemonId id)
        {
            return PokemonDict[id].Clone();
        }

        public IMove Retrieve(MoveId id)
        {
            return MoveDict[id].Clone();
        }

        public IAbility Retrieve(AbilityId id)
        {
            return AbilityDict[id].Clone();
        }

        public ISaihai Retrieve(SaihaiId id)
        {
            return SaihaiDict[id].Clone();
        }

        public IGimmick Retrieve(GimmickId id)
        {
            return GimmickDict[id].Clone();
        }

        public IBuilding Retrieve(BuildingId id)
        {
            return BuildingDict[id].Clone();
        }

        public void Save(PokemonId id, IPokemon model)
        {
            PokemonDict[id] = model.Clone();
        }

        public void Save(MoveId id, IMove model)
        {
            MoveDict[id] = model.Clone();
        }

        public void Save(AbilityId id, IAbility model)
        {
            AbilityDict[id] = model.Clone();
        }

        public void Save(SaihaiId id, ISaihai model)
        {
            SaihaiDict[id] = model.Clone();
        }

        public void Save(GimmickId id, IGimmick model)
        {
            GimmickDict[id] = model.Clone();
        }

        public void Save(BuildingId id, IBuilding model)
        {
            BuildingDict[id] = model.Clone();
        }
    }
}
