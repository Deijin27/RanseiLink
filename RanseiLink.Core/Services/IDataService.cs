using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services;

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
    IMaxLinkService MaxLink { get; }
    IBaseWarriorService BaseWarrior { get; }
    IScenarioAppearPokemonService ScenarioAppearPokemon { get; }
    IScenarioKingdomService ScenarioKingdom { get; }
}
