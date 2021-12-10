using RanseiLink.Core.Enums;
using RanseiLink.ViewModels;
using System.Linq;

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
        if (_mainEditor.AbilitySelector.Items.Contains(id))
        {
            _mainEditor.AbilitySelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.Ability;
        }
    }

    public void JumpToBaseWarrior(WarriorId id)
    {
        if (_mainEditor.BaseWarriorSelector.Items.Contains(id))
        {
            _mainEditor.BaseWarriorSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.BaseWarrior;
        }
    }

    public void JumpToBuilding(BuildingId id)
    {
        if (_mainEditor.BuildingSelector.Items.Contains(id))
        {
            _mainEditor.BuildingSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.Building;
        }
    }

    public void JumpToEventSpeaker(EventSpeakerId id)
    {
        if (_mainEditor.EventSpeakerSelector.Items.Contains(id))
        {
            _mainEditor.EventSpeakerSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.EventSpeaker;
        }
    }

    public void JumpToEvolutionTable()
    {
        _mainEditor.CurrentPage = MainEditorPage.EvolutionTable;
    }

    public void JumpToItem(ItemId id)
    {
        if (_mainEditor.ItemSelector.Items.Contains(id))
        {
            _mainEditor.ItemSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.Item;
        }
    }

    public void JumpToMaxLink(WarriorId id)
    {
        if (_mainEditor.MaxLinkSelector.Items.Contains(id))
        {
            _mainEditor.MaxLinkSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.MaxLink;
        }
    }

    public void JumpToMove(MoveId id)
    {
        if (_mainEditor.MoveSelector.Items.Contains(id))
        {
            _mainEditor.MoveSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.Move;
        }
    }

    public void JumpToMoveRange(MoveRangeId id)
    {
        if (_mainEditor.MoveRangeSelector.Items.Contains(id))
        {
            _mainEditor.MoveRangeSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.MoveRange;
        }
    }

    public void JumpToPokemon(PokemonId id)
    {
        if (_mainEditor.PokemonSelector.Items.Contains(id))
        {
            _mainEditor.PokemonSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.Pokemon;
        }
    }

    public void JumpToScenarioAppearPokemon(ScenarioId id)
    {
        if (_mainEditor.ScenarioAppearPokemonSelector.Items.Contains(id))
        {
            _mainEditor.ScenarioAppearPokemonSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.ScenarioAppearPokemon;
        }
    }

    public void JumpToScenarioKingdom(ScenarioId id)
    {
        if (_mainEditor.ScenarioKingdomSelector.Items.Contains(id))
        {
            _mainEditor.ScenarioKingdomSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.ScenarioKingdom;
        }
    }

    public void JumpToScenarioPokemon(ScenarioId scenario, uint id)
    {
        var selector = _mainEditor.ScenarioPokemonSelector;
        if (selector.ScenarioItems.Contains(scenario)
            && selector.MinIndex <= id
            && selector.MaxIndex >= id)
        {
            selector.SelectedScenario = scenario;
            selector.SelectedItem = id;
            _mainEditor.CurrentPage = MainEditorPage.ScenarioPokemon;
        }
    }

    public void JumpToScenarioWarrior(ScenarioId scenario, uint id)
    {
        var selector = _mainEditor.ScenarioWarriorSelector;
        if (selector.ScenarioItems.Contains(scenario)
            && selector.MinIndex <= id
            && selector.MaxIndex >= id)
        {
            selector.SelectedScenario = scenario;
            selector.SelectedItem = id;
            _mainEditor.CurrentPage = MainEditorPage.ScenarioWarrior;
        }
    }

    public void JumpToWarriorNameTable()
    {
        _mainEditor.CurrentPage = MainEditorPage.WarriorNameTable;
    }

    public void JumpToWarriorSkill(WarriorSkillId id)
    {
        if (_mainEditor.WarriorSkillSelector.Items.Contains(id))
        {
            _mainEditor.WarriorSkillSelector.Selected = id;
            _mainEditor.CurrentPage = MainEditorPage.WarriorSkill;
        }
    }
}
