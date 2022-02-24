using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Tests.Mocks;

internal class MockDataService : IModServiceContainer
{
    public IPokemonService Pokemon { get; set; }

    public IMoveService Move { get; set; }

    public IAbilityService Ability { get; set; }

    public IWarriorSkillService WarriorSkill { get; set; }

    public IGimmickService Gimmick { get; set; }

    public IBuildingService Building { get; set; }

    public IItemService Item { get; set; }

    public IKingdomService Kingdom { get; set; }

    public IMoveRangeService MoveRange { get; set; }

    public IEventSpeakerService EventSpeaker { get; set; }

    public IScenarioPokemonService ScenarioPokemon { get; set; }

    public IScenarioWarriorService ScenarioWarrior { get; set; }

    public IMaxLinkService MaxLink { get; set; }

    public IBaseWarriorService BaseWarrior { get; set; }

    public IScenarioAppearPokemonService ScenarioAppearPokemon { get; set; }

    public IScenarioKingdomService ScenarioKingdom { get; set; }

    public IMsgBlockService Msg { get; set; }

    public IBattleConfigService BattleConfig { get; set; }

    public IMapIdService MapName { get; set; }

    public IGimmickRangeService GimmickRange { get; set; }

    public IMoveAnimationService MoveAnimation { get; set; }

    public IGimmickObjectService GimmickObject { get; set; }

    public IMapService Map { get; set; }

    public IOverrideSpriteProvider OverrideSpriteProvider { get; set; }
}
