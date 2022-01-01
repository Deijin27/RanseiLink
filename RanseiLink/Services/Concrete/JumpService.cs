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
        var moduleId = AbilitySelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is AbilitySelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToBaseWarrior(WarriorId id)
    {
        var moduleId = BaseWarriorSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is BaseWarriorSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToBuilding(BuildingId id)
    {
        var moduleId = BuildingSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is BuildingSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToEventSpeaker(EventSpeakerId id)
    {
        var moduleId = EventSpeakerSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is EventSpeakerSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToEvolutionTable()
    {
        _mainEditor.CurrentPage = EvolutionTableEditorModule.Id;
    }

    public void JumpToItem(ItemId id)
    {
        var moduleId = ItemSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is ItemSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToMaxLink(WarriorId id)
    {
        var moduleId = MaxLinkSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is MaxLinkSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToMove(MoveId id)
    {
        var moduleId = MoveSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is MoveSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToMoveRange(MoveRangeId id)
    {
        var moduleId = MoveRangeSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is MoveRangeSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToPokemon(PokemonId id)
    {
        var moduleId = PokemonSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is PokemonSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToScenarioAppearPokemon(ScenarioId id)
    {
        var moduleId = ScenarioAppearPokemonSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is ScenarioAppearPokemonSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToScenarioKingdom(ScenarioId id)
    {
        var moduleId = ScenarioKingdomSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is ScenarioKingdomSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToScenarioPokemon(ScenarioId scenario, uint id)
    {
        var moduleId = ScenarioPokemonSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is ScenarioPokemonSelectorViewModel selectorVm 
            && selectorVm.ScenarioItems.Contains(scenario)
            && selectorVm.MinIndex <= id
            && selectorVm.MaxIndex >= id)
        {
            selectorVm.SelectedScenario = scenario;
            selectorVm.SelectedItem = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToScenarioWarrior(ScenarioId scenario, uint id)
    {
        var moduleId = ScenarioWarriorSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is ScenarioWarriorSelectorViewModel selectorVm
            && selectorVm.ScenarioItems.Contains(scenario)
            && selectorVm.MinIndex <= id
            && selectorVm.MaxIndex >= id)
        {
            selectorVm.SelectedScenario = scenario;
            selectorVm.SelectedItem = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToWarriorNameTable()
    {
        _mainEditor.CurrentPage = WarriorNameTableEditorModule.Id;
    }

    public void JumpToWarriorSkill(WarriorSkillId id)
    {
        var moduleId = WarriorSkillSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is WarriorSkillSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }

    public void JumpToBattleConfig(BattleConfigId id)
    {
        var moduleId = BattleConfigSelectorEditorModule.Id;
        if (_mainEditor.ViewModels[moduleId] is BattleConfigSelectorViewModel selectorVm && selectorVm.Items.Contains(id))
        {
            selectorVm.Selected = id;
            _mainEditor.CurrentPage = moduleId;
        }
    }
}
