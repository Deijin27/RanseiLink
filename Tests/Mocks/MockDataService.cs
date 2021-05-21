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
        public Dictionary<WarriorSkillId, IWarriorSkill> WarriorSkillDict = new Dictionary<WarriorSkillId, IWarriorSkill>();
        public Dictionary<GimmickId, IGimmick> GimmickDict = new Dictionary<GimmickId, IGimmick>();
        public Dictionary<BuildingId, IBuilding> BuildingDict = new Dictionary<BuildingId, IBuilding>();
        public Dictionary<ItemId, IItem> ItemDict = new Dictionary<ItemId, IItem>();
        public Dictionary<KingdomId, IKingdom> KingdomDict = new Dictionary<KingdomId, IKingdom>();
        public Dictionary<MoveRangeId, IMoveRange> MoveRangeDict = new Dictionary<MoveRangeId, IMoveRange>();
        public Dictionary<EventSpeakerId, IEventSpeaker> EventSpeakerDict = new Dictionary<EventSpeakerId, IEventSpeaker>();


        public Dictionary<PokemonId, IPokemon> AllPokemon()
        {
            var dict = new Dictionary<PokemonId, IPokemon>();
            foreach (var (key, value) in PokemonDict)
            {
                dict[key] = value.Clone();
            }
            return dict;
        }

        public Queue<string> CommitRomCallLog = new Queue<string>();
        public void CommitToRom(string path)
        {
            CommitRomCallLog.Enqueue(path);
        }
        public Queue<string> LoadRomCallLog = new Queue<string>();
        public void LoadRom(string path)
        {
            LoadRomCallLog.Enqueue(path);
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

        public IWarriorSkill Retrieve(WarriorSkillId id)
        {
            return WarriorSkillDict[id].Clone();
        }

        public IGimmick Retrieve(GimmickId id)
        {
            return GimmickDict[id].Clone();
        }

        public IBuilding Retrieve(BuildingId id)
        {
            return BuildingDict[id].Clone();
        }

        public IItem Retrieve(ItemId id)
        {
            return ItemDict[id].Clone();
        }

        public IKingdom Retrieve(KingdomId id)
        {
            return KingdomDict[id].Clone();
        }

        public IMoveRange Retrieve(MoveRangeId id)
        {
            return MoveRangeDict[id].Clone();
        }

        public IEventSpeaker Retrieve(EventSpeakerId id)
        {
            return EventSpeakerDict[id].Clone();
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

        public void Save(WarriorSkillId id, IWarriorSkill model)
        {
            WarriorSkillDict[id] = model.Clone();
        }

        public void Save(GimmickId id, IGimmick model)
        {
            GimmickDict[id] = model.Clone();
        }

        public void Save(BuildingId id, IBuilding model)
        {
            BuildingDict[id] = model.Clone();
        }

        public void Save(ItemId id, IItem model)
        {
            ItemDict[id] = model.Clone();
        }

        public void Save(KingdomId id, IKingdom model)
        {
            KingdomDict[id] = model.Clone();
        }

        public void Save(MoveRangeId id, IMoveRange model)
        {
            MoveRangeDict[id] = model.Clone();
        }

        public void Save(EventSpeakerId id, IEventSpeaker model)
        {
            EventSpeakerDict[id] = model.Clone();
        }
    }
}
