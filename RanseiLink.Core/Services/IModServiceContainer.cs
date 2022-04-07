using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services;

/// <summary>
/// Use for lua scripts only
/// </summary>
public interface IModServiceContainer
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
    IMsgBlockService Msg { get; }
    IBattleConfigService BattleConfig { get; }
    IMapService Map { get; }
    IGimmickRangeService GimmickRange { get; }
    IMoveAnimationService MoveAnimation { get; }
    IGimmickObjectService GimmickObject { get; }
    IEpisodeService Episode { get; }
    IOverrideSpriteProvider OverrideSpriteProvider { get; }
}
