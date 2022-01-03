using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services.Concrete;

public class DataService : IDataService
{
    public DataService(ModInfo mod, IServiceContainer container)
    {
        var msgService = container.Resolve<IMsgService>();

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
        ScenarioPokemon = new ScenarioPokemonService(mod);
        ScenarioWarrior = new ScenarioWarriorService(mod);
        MaxLink = new MaxLinkService(mod);
        BaseWarrior = new BaseWarriorService(mod);
        ScenarioAppearPokemon = new ScenarioAppearPokemonService(mod);
        ScenarioKingdom = new ScenarioKingdomService(mod);
        Msg = new MsgBlockService(mod, msgService);
        BattleConfig = new BattleConfigService(mod);
        BattleEnvironment = new BattleEnvironmentService(mod);
        GimmickRange = new GimmickRangeService(mod);
    }

    public IPokemonService Pokemon { get; }
    public IMoveService Move { get; }
    public IAbilityService Ability { get; }
    public IWarriorSkillService WarriorSkill { get; }
    public IGimmickService Gimmick { get; }
    public IBuildingService Building { get; }
    public IItemService Item { get; }
    public IKingdomService Kingdom { get; }
    public IMoveRangeService MoveRange { get; }
    public IGimmickRangeService GimmickRange { get; }
    public IEventSpeakerService EventSpeaker { get; }
    public IScenarioPokemonService ScenarioPokemon { get; }
    public IScenarioWarriorService ScenarioWarrior { get; }
    public IMaxLinkService MaxLink { get; }
    public IBaseWarriorService BaseWarrior { get; }
    public IScenarioAppearPokemonService ScenarioAppearPokemon { get; }
    public IScenarioKingdomService ScenarioKingdom { get; }
    public IMsgBlockService Msg { get; }
    public IBattleConfigService BattleConfig { get; }

    public IBattleEnvironmentService BattleEnvironment { get; }
}
