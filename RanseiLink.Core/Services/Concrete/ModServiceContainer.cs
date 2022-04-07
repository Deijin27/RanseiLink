using RanseiLink.Core.Services.ModelServices;
using System;

namespace RanseiLink.Core.Services.Concrete;

public class ModServiceContainer : IModServiceContainer
{
    private readonly Lazy<IPokemonService> _pokemon;
    private readonly Lazy<IMoveService> _move;
    private readonly Lazy<IAbilityService> _ability;
    private readonly Lazy<IWarriorSkillService> _warriorSkill;
    private readonly Lazy<IGimmickService> _gimmick;
    private readonly Lazy<IBuildingService> _building;
    private readonly Lazy<IItemService> _item;
    private readonly Lazy<IKingdomService> _kingdom;
    private readonly Lazy<IMoveRangeService> _moveRange;
    private readonly Lazy<IGimmickRangeService> _gimmickRange;
    private readonly Lazy<IEventSpeakerService> _eventSpeaker;
    private readonly Lazy<IScenarioPokemonService> _scenarioPokemon;
    private readonly Lazy<IScenarioWarriorService> _scenarioWarrior;
    private readonly Lazy<IMaxLinkService> _maxLink;
    private readonly Lazy<IBaseWarriorService> _baseWarrior;
    private readonly Lazy<IScenarioAppearPokemonService> _scenarioAppearPokemon;
    private readonly Lazy<IScenarioKingdomService> _scenarioKingdom;
    private readonly Lazy<IMsgBlockService> _msg;
    private readonly Lazy<IBattleConfigService> _battleConfig;
    private readonly Lazy<IMoveAnimationService> _moveAnimation;
    private readonly Lazy<IGimmickObjectService> _gimmickObject;
    private readonly Lazy<IMapService> _map;
    private readonly Lazy<IOverrideSpriteProvider> _overrideSpriteProvider;
    private readonly Lazy<IEpisodeService> _episode;

    public ModServiceContainer(IServiceGetter modServices)
    {
        _pokemon = new(() => modServices.Get<IPokemonService>());
        _move = new(() => modServices.Get<IMoveService>());
        _ability = new(() => modServices.Get<IAbilityService>());
        _warriorSkill = new(() => modServices.Get<IWarriorSkillService>());
        _gimmick = new(() => modServices.Get<IGimmickService>());
        _building = new(() => modServices.Get<IBuildingService>());
        _item = new(() => modServices.Get<IItemService>());
        _kingdom = new(() => modServices.Get<IKingdomService>());
        _moveRange = new(() => modServices.Get<IMoveRangeService>());
        _gimmickRange = new(() => modServices.Get<IGimmickRangeService>());
        _eventSpeaker = new(() => modServices.Get<IEventSpeakerService>());
        _scenarioPokemon = new(() => modServices.Get<IScenarioPokemonService>());
        _scenarioWarrior = new(() => modServices.Get<IScenarioWarriorService>());
        _maxLink = new(() => modServices.Get<IMaxLinkService>());
        _baseWarrior = new(() => modServices.Get<IBaseWarriorService>());
        _scenarioAppearPokemon = new(() => modServices.Get<IScenarioAppearPokemonService>());
        _scenarioKingdom = new(() => modServices.Get<IScenarioKingdomService>());
        _msg = new(() => modServices.Get<IMsgBlockService>());
        _battleConfig = new(() => modServices.Get<IBattleConfigService>());
        _moveAnimation = new(() => modServices.Get<IMoveAnimationService>());
        _gimmickObject = new(() => modServices.Get<IGimmickObjectService>());
        _map = new(() => modServices.Get<IMapService>());
        _overrideSpriteProvider = new(() => modServices.Get<IOverrideSpriteProvider>());
        _episode = new(() => modServices.Get<IEpisodeService>());
    }

    public IPokemonService Pokemon => _pokemon.Value;
    public IMoveService Move => _move.Value;
    public IAbilityService Ability => _ability.Value;
    public IWarriorSkillService WarriorSkill => _warriorSkill.Value;
    public IGimmickService Gimmick => _gimmick.Value;
    public IBuildingService Building => _building.Value;
    public IItemService Item => _item.Value;
    public IKingdomService Kingdom => _kingdom.Value;
    public IMoveRangeService MoveRange => _moveRange.Value;
    public IGimmickRangeService GimmickRange => _gimmickRange.Value;
    public IEventSpeakerService EventSpeaker => _eventSpeaker.Value;
    public IScenarioPokemonService ScenarioPokemon => _scenarioPokemon.Value;
    public IScenarioWarriorService ScenarioWarrior => _scenarioWarrior.Value;
    public IMaxLinkService MaxLink => _maxLink.Value;
    public IBaseWarriorService BaseWarrior => _baseWarrior.Value;
    public IScenarioAppearPokemonService ScenarioAppearPokemon => _scenarioAppearPokemon.Value;
    public IScenarioKingdomService ScenarioKingdom => _scenarioKingdom.Value;
    public IMsgBlockService Msg => _msg.Value;
    public IBattleConfigService BattleConfig => _battleConfig.Value;
    public IMoveAnimationService MoveAnimation => _moveAnimation.Value;
    public IGimmickObjectService GimmickObject => _gimmickObject.Value;
    public IMapService Map => _map.Value;
    public IOverrideSpriteProvider OverrideSpriteProvider => _overrideSpriteProvider.Value;
    public IEpisodeService Episode => _episode.Value;
}
