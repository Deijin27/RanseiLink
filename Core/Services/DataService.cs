using Core.Enums;
using Core.Models.Interfaces;
using Core.Services.ModelServices;

namespace Core.Services
{
    public class DataService : IDataService
    {
        public DataService(ModInfo mod)
        {
            Pokemon = new PokemonService(mod);
            Move = new MoveService(mod);
            Ability = new AbilityService(mod);
            WarriorSkill = new WarriorSkillService(mod);
            Gimmick = new GimmickService(mod);
            Building = new BuildingService(mod);
            Item = new ItemService(mod);
            Kingdom = new KingdomService(mod);
            MoveRange = new MoveRangeService(mod);
            EventSpeaker = new EventSpeakerService(mod);
            EvolutionTable = new EvolutionTableService(mod);
            EvolutionTable = new EvolutionTableService(mod);
            ScenarioPokemon = new ScenarioPokemonService(mod);
            ScenarioWarrior = new ScenarioWarriorService(mod);
            MaxLink = new WarriorMaxSyncService(mod);
        }

        public IPokemonService Pokemon { get; }
        public IModelDataService<MoveId, IMove> Move { get; }
        public IModelDataService<AbilityId, IAbility> Ability { get; }
        public IModelDataService<WarriorSkillId, IWarriorSkill> WarriorSkill { get; }
        public IModelDataService<GimmickId, IGimmick> Gimmick { get; }
        public IModelDataService<BuildingId, IBuilding> Building { get; }
        public IModelDataService<ItemId, IItem> Item { get; }
        public IModelDataService<KingdomId, IKingdom> Kingdom { get; }
        public IModelDataService<MoveRangeId, IMoveRange> MoveRange { get; }
        public IModelDataService<EventSpeakerId, IEventSpeaker> EventSpeaker { get; }
        public IModelDataService<IEvolutionTable> EvolutionTable { get; }
        public IScenarioPokemonService ScenarioPokemon { get; }
        public IScenarioWarriorService ScenarioWarrior { get; }
        public IWarriorMaxSyncService MaxLink { get; }
    }
}
