using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Tests.Mocks
{
    internal class MockDataService : IDataService
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

        public IWarriorMaxLinkService MaxLink { get; set; }

        public IBaseWarriorService BaseWarrior { get; set; }

        public IScenarioAppearPokemonService ScenarioAppearPokemon { get; set; }

        public IScenarioKingdomService ScenarioKingdom { get; set; }
    }
}
