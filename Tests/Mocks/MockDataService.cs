using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using Core.Services.ModelServices;
using System;
using System.Collections.Generic;

namespace Tests.Mocks
{
    internal class MockDataService : IDataService
    {
        public Dictionary<PokemonId, IPokemon> PokemonDict = new Dictionary<PokemonId, IPokemon>();

        public Queue<string> CommitRomCallLog = new Queue<string>();
        public void CommitToRom(string path)
        {
            CommitRomCallLog.Enqueue(path);
        }

        public IPokemon Retrieve(PokemonId id)
        {
            return PokemonDict[id].Clone();
        }

        public IPokemonService Pokemon { get; set; }

        public IMoveService Move => throw new NotImplementedException();

        public IAbilityService Ability => throw new NotImplementedException();

        public IWarriorSkillService WarriorSkill => throw new NotImplementedException();

        public IGimmickService Gimmick => throw new NotImplementedException();

        public IBuildingService Building => throw new NotImplementedException();

        public IItemService Item => throw new NotImplementedException();

        public IKingdomService Kingdom => throw new NotImplementedException();

        public IMoveRangeService MoveRange => throw new NotImplementedException();

        public IEventSpeakerService EventSpeaker => throw new NotImplementedException();

        public IEvolutionTableService EvolutionTable => throw new NotImplementedException();

        public IScenarioPokemonService ScenarioPokemon => throw new NotImplementedException();

        public IScenarioWarriorService ScenarioWarrior => throw new NotImplementedException();

        public IWarriorMaxLinkService MaxLink => throw new NotImplementedException();

        
    }
}
