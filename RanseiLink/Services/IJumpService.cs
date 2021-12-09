using RanseiLink.Core.Enums;

namespace RanseiLink.Services;

public interface IJumpService
{
    void JumpToAbility(AbilityId id);
    void JumpToBaseWarrior(WarriorId id);
    void JumpToBuilding(BuildingId id);
    void JumpToEventSpeaker(EventSpeakerId id);
    void JumpToEvolutionTable();
    void JumpToItem(ItemId id);
    void JumpToMaxLink(WarriorId id);
    void JumpToMoveRange(MoveRangeId id);
    void JumpToMove(MoveId id);
    void JumpToPokemon(PokemonId id);
    void JumpToScenarioAppearPokemon(ScenarioId scenario);
    void JumpToScenarioKingdom(ScenarioId id);
    void JumpToScenarioWarrior(ScenarioId scenario, uint id);
    void JumpToScenarioPokemon(ScenarioId scenario, uint id);
    void JumpToWarriorNameTable();
    void JumpToWarriorSkill(WarriorSkillId id);
}
