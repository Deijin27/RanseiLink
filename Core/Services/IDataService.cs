using Core.Services.ModelServices;

namespace Core.Services
{
    public interface IDataService
    {
        IPokemonService Pokemon { get; }
        IMoveService Move { get; }
        IAbilityService Ability { get; }
        IWarriorSkillService WarriorSkill { get; }
        IGimmickService Gimmick { get; }
        IBuildingService Building { get; }
        IItemService Item { get; }
        IKingdomService Kingdom { get; }
        IMoveRangeService MoveRange { get; }
        IEventSpeakerService EventSpeaker { get; }
        IScenarioPokemonService ScenarioPokemon { get; }
        IScenarioWarriorService ScenarioWarrior { get; }
        IWarriorMaxLinkService MaxLink { get; }
    }
}