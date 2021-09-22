using Core.Enums;
using Core.Models.Interfaces;
using Core.Services.ModelServices;

namespace Core.Services
{
    public interface IDataService
    {
        IPokemonService Pokemon { get; }
        IModelDataService<MoveId, IMove> Move { get; }
        IModelDataService<AbilityId, IAbility> Ability { get; }
        IModelDataService<WarriorSkillId, IWarriorSkill> WarriorSkill { get; }
        IModelDataService<GimmickId, IGimmick> Gimmick { get; }
        IModelDataService<BuildingId, IBuilding> Building { get; }
        IModelDataService<ItemId, IItem> Item { get; }
        IModelDataService<KingdomId, IKingdom> Kingdom { get; }
        IModelDataService<MoveRangeId, IMoveRange> MoveRange { get; }
        IModelDataService<EventSpeakerId, IEventSpeaker> EventSpeaker { get; }
        IModelDataService<IEvolutionTable> EvolutionTable { get; }
        IScenarioPokemonService ScenarioPokemon { get; }
        IWarriorMaxSyncService MaxLink { get; }
    }
}