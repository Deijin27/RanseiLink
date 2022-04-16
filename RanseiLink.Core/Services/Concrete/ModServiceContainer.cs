using RanseiLink.Core.Services.ModelServices;
using System;

namespace RanseiLink.Core.Services.Concrete
{
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
            _pokemon = new Lazy<IPokemonService>(() => modServices.Get<IPokemonService>());
            _move = new Lazy<IMoveService>(() => modServices.Get<IMoveService>());
            _ability = new Lazy<IAbilityService>(() => modServices.Get<IAbilityService>());
            _warriorSkill = new Lazy<IWarriorSkillService>(() => modServices.Get<IWarriorSkillService>());
            _gimmick = new Lazy<IGimmickService>(() => modServices.Get<IGimmickService>());
            _building = new Lazy<IBuildingService>(() => modServices.Get<IBuildingService>());
            _item = new Lazy<IItemService>(() => modServices.Get<IItemService>());
            _kingdom = new Lazy<IKingdomService>(() => modServices.Get<IKingdomService>());
            _moveRange = new Lazy<IMoveRangeService>(() => modServices.Get<IMoveRangeService>());
            _gimmickRange = new Lazy<IGimmickRangeService>(() => modServices.Get<IGimmickRangeService>());
            _eventSpeaker = new Lazy<IEventSpeakerService>(() => modServices.Get<IEventSpeakerService>());
            _scenarioPokemon = new Lazy<IScenarioPokemonService>(() => modServices.Get<IScenarioPokemonService>());
            _scenarioWarrior = new Lazy<IScenarioWarriorService>(() => modServices.Get<IScenarioWarriorService>());
            _maxLink = new Lazy<IMaxLinkService>(() => modServices.Get<IMaxLinkService>());
            _baseWarrior = new Lazy<IBaseWarriorService>(() => modServices.Get<IBaseWarriorService>());
            _scenarioAppearPokemon = new Lazy<IScenarioAppearPokemonService>(() => modServices.Get<IScenarioAppearPokemonService>());
            _scenarioKingdom = new Lazy<IScenarioKingdomService>(() => modServices.Get<IScenarioKingdomService>());
            _msg = new Lazy<IMsgBlockService>(() => modServices.Get<IMsgBlockService>());
            _battleConfig = new Lazy<IBattleConfigService>(() => modServices.Get<IBattleConfigService>());
            _moveAnimation = new Lazy<IMoveAnimationService>(() => modServices.Get<IMoveAnimationService>());
            _gimmickObject = new Lazy<IGimmickObjectService>(() => modServices.Get<IGimmickObjectService>());
            _map = new Lazy<IMapService>(() => modServices.Get<IMapService>());
            _overrideSpriteProvider = new Lazy<IOverrideSpriteProvider>(() => modServices.Get<IOverrideSpriteProvider>());
            _episode = new Lazy<IEpisodeService>(() => modServices.Get<IEpisodeService>());
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
}
