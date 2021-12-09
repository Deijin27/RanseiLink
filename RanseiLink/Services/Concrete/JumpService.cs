using RanseiLink.Core.Enums;
using RanseiLink.ViewModels;

namespace RanseiLink.Services.Concrete;

internal class JumpService : IJumpService 
{ 
    private readonly MainEditorViewModel _mainEditor;

    public JumpService(MainEditorViewModel mainEditorViewModel)
    {
        _mainEditor = mainEditorViewModel;
    }

    public void JumpToAbility(AbilityId id)
    {
        _mainEditor.AbilitySelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.AbilitySelector;
    }

    public void JumpToBaseWarrior(WarriorId id)
    {
        _mainEditor.BaseWarriorSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.BaseWarriorSelector;
    }

    public void JumpToBuilding(BuildingId id)
    {
        _mainEditor.BuildingSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.BuildingSelector;
    }

    public void JumpToEventSpeaker(EventSpeakerId id)
    {
        _mainEditor.EventSpeakerSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.EventSpeakerSelector;
    }

    public void JumpToEvolutionTable()
    {
        _mainEditor.CurrentVm = _mainEditor.EvolutionTableViewModel;
    }

    public void JumpToItem(ItemId id)
    {
        _mainEditor.ItemSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.ItemSelector;
    }

    public void JumpToMaxLink(WarriorId id)
    {
        _mainEditor.MaxLinkSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.MaxLinkSelector;
    }

    public void JumpToMove(MoveId id)
    {
        _mainEditor.MoveSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.MoveSelector;
    }

    public void JumpToMoveRange(MoveRangeId id)
    {
        _mainEditor.MoveRangeSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.MoveRangeSelector;
    }

    public void JumpToPokemon(PokemonId id)
    {
        _mainEditor.PokemonSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.PokemonSelector;
    }

    public void JumpToScenarioAppearPokemon(ScenarioId id)
    {
        _mainEditor.ScenarioAppearPokemonSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.ScenarioAppearPokemonSelector;
    }

    public void JumpToScenarioKingdom(ScenarioId id)
    {
        _mainEditor.ScenarioKingdomSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.ScenarioKingdomSelector;
    }

    public void JumpToScenarioPokemon(ScenarioId scenario, uint id)
    {
        _mainEditor.ScenarioPokemonSelector.SelectedScenario = scenario;
        _mainEditor.ScenarioPokemonSelector.SelectedItem = id;
        _mainEditor.CurrentVm = _mainEditor.ScenarioPokemonSelector;
    }

    public void JumpToScenarioWarrior(ScenarioId scenario, uint id)
    {
        _mainEditor.ScenarioWarriorSelector.SelectedScenario = scenario;
        _mainEditor.ScenarioWarriorSelector.SelectedItem = id;
        _mainEditor.CurrentVm = _mainEditor.ScenarioWarriorSelector;
    }

    public void JumpToWarriorNameTable()
    {
        _mainEditor.CurrentVm = _mainEditor.WarriorNameTableViewModel;
    }

    public void JumpToWarriorSkill(WarriorSkillId id)
    {
        _mainEditor.WarriorSkillSelector.Selected = id;
        _mainEditor.CurrentVm = _mainEditor.WarriorSkillSelector;
    }
}
