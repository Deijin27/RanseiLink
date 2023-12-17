using RanseiLink.Core.Services.ModelServices;
using System;

namespace RanseiLink.Core.Services.Concrete
{
    public class ModServiceContainer(IServiceGetter modServices) : IModServiceContainer
    {
        private readonly Lazy<IPokemonService> _pokemon = new(modServices.Get<IPokemonService>);
        private readonly Lazy<IMoveService> _move = new(modServices.Get<IMoveService>);
        private readonly Lazy<IAbilityService> _ability = new(modServices.Get<IAbilityService>);
        private readonly Lazy<IWarriorSkillService> _warriorSkill = new(modServices.Get<IWarriorSkillService>);
        private readonly Lazy<IGimmickService> _gimmick = new(modServices.Get<IGimmickService>);
        private readonly Lazy<IBuildingService> _building = new(modServices.Get<IBuildingService>);
        private readonly Lazy<IItemService> _item = new(modServices.Get<IItemService>);
        private readonly Lazy<IKingdomService> _kingdom = new(modServices.Get<IKingdomService>);
        private readonly Lazy<IMoveRangeService> _moveRange = new(modServices.Get<IMoveRangeService>);
        private readonly Lazy<IGimmickRangeService> _gimmickRange = new(modServices.Get<IGimmickRangeService>);
        private readonly Lazy<IEventSpeakerService> _eventSpeaker = new(modServices.Get<IEventSpeakerService>);
        private readonly Lazy<IScenarioPokemonService> _scenarioPokemon = new(modServices.Get<IScenarioPokemonService>);
        private readonly Lazy<IScenarioWarriorService> _scenarioWarrior = new(modServices.Get<IScenarioWarriorService>);
        private readonly Lazy<IMaxLinkService> _maxLink = new(modServices.Get<IMaxLinkService>);
        private readonly Lazy<IBaseWarriorService> _baseWarrior = new(modServices.Get<IBaseWarriorService>);
        private readonly Lazy<IScenarioAppearPokemonService> _scenarioAppearPokemon = new(modServices.Get<IScenarioAppearPokemonService>);
        private readonly Lazy<IScenarioKingdomService> _scenarioKingdom = new(modServices.Get<IScenarioKingdomService>);
        private readonly Lazy<IScenarioBuildingService> _scenarioBuilding = new(modServices.Get<IScenarioBuildingService>);
        private readonly Lazy<IMsgBlockService> _msg = new(modServices.Get<IMsgBlockService>);
        private readonly Lazy<IBattleConfigService> _battleConfig = new(modServices.Get<IBattleConfigService>);
        private readonly Lazy<IMoveAnimationService> _moveAnimation = new(modServices.Get<IMoveAnimationService>);
        private readonly Lazy<IGimmickObjectService> _gimmickObject = new(modServices.Get<IGimmickObjectService>);
        private readonly Lazy<IMapService> _map = new(modServices.Get<IMapService>);
        private readonly Lazy<IOverrideDataProvider> _overrideSpriteProvider = new(modServices.Get<IOverrideDataProvider>);
        private readonly Lazy<IEpisodeService> _episode = new(modServices.Get<IEpisodeService>);

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
        public IScenarioBuildingService ScenarioBuilding => _scenarioBuilding.Value;
        public IMsgBlockService Msg => _msg.Value;
        public IBattleConfigService BattleConfig => _battleConfig.Value;
        public IMoveAnimationService MoveAnimation => _moveAnimation.Value;
        public IGimmickObjectService GimmickObject => _gimmickObject.Value;
        public IMapService Map => _map.Value;
        public IOverrideDataProvider OverrideSpriteProvider => _overrideSpriteProvider.Value;
        public IEpisodeService Episode => _episode.Value;
    } 
}
