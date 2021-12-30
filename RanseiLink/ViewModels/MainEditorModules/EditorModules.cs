using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public class PokemonSelectorEditorModule : IEditorModule
{
    public const string Id = "pokemon_selector";
    public string UniqueId => Id;
    public string ListName => "Pokemon";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new PokemonSelectorViewModel(container, context);
}

public class PokemonGridEditorModule : IEditorModule
{
    public const string Id = "pokemon_grid";
    public string UniqueId => Id;
    public string ListName => "Pokemon (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new PokemonGridViewModel(container, context);
}

public class AbilitySelectorEditorModule : IEditorModule
{
    public const string Id = "ability_selector";
    public string UniqueId => Id;
    public string ListName => "Ability";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new AbilitySelectorViewModel(container, context);
}

public class AbilityGridEditorModule : IEditorModule
{
    public const string Id = "ability_grid";
    public string UniqueId => Id;
    public string ListName => "Ability (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new AbilityGridViewModel(container, context);
}

public class WarriorSkillSelectorEditorModule : IEditorModule
{
    public const string Id = "warrior_skill_selector";
    public string UniqueId => Id;
    public string ListName => "Warrior Skill";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new WarriorSkillSelectorViewModel(container, context);
}

public class WarriorSkillGridEditorModule : IEditorModule
{
    public const string Id = "warrior_skill_grid";
    public string UniqueId => Id;
    public string ListName => "Warrior Skill (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new WarriorSkillGridViewModel(container, context);
}

public class MoveRangeSelectorEditorModule : IEditorModule
{
    public const string Id = "move_range_selector";
    public string UniqueId => Id;
    public string ListName => "Move Range";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new MoveRangeSelectorViewModel(container, context);
}

public class MoveSelectorEditorModule : IEditorModule
{
    public const string Id = "move_selector";
    public string UniqueId => Id;
    public string ListName => "Move";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new MoveSelectorViewModel(container, context);
}

public class MoveGridEditorModule : IEditorModule
{
    public const string Id = "move_grid";
    public string UniqueId => Id;
    public string ListName => "Move (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new MoveGridViewModel(container, context);
}

public class EvolutionTableEditorModule : IEditorModule
{
    public const string Id = "evolution_table";
    public string UniqueId => Id;
    public string ListName => "Evolution Table";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new EvolutionTableViewModel(container, context);
}

public class WarriorNameTableEditorModule : IEditorModule
{
    public const string Id = "warrior_name_table";
    public string UniqueId => Id;
    public string ListName => "Warrior Name Table";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new WarriorNameTableViewModel(container, context);
}

public class BaseWarriorSelectorEditorModule : IEditorModule
{
    public const string Id = "base_warrior_selector";
    public string UniqueId => Id;
    public string ListName => "Base Warrior";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new BaseWarriorSelectorViewModel(container, context);
}

public class BaseWarriorGridEditorModule : IEditorModule
{
    public const string Id = "base_warrior_grid";
    public string UniqueId => Id;
    public string ListName => "Base Warrior (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new BaseWarriorGridViewModel(container, context);
}
public class MaxLinkSelectorEditorModule : IEditorModule
{
    public const string Id = "max_link_selector";
    public string UniqueId => Id;
    public string ListName => "Max Link";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new MaxLinkSelectorViewModel(container, context);
}

public class ScenarioWarriorSelectorEditorModule : IEditorModule
{
    public const string Id = "scenario_warrior_selector";
    public string UniqueId => Id;
    public string ListName => "Scenario Warrior";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ScenarioWarriorSelectorViewModel(container, context);
}

public class ScenarioWarriorGridEditorModule : IEditorModule
{
    public const string Id = "scenario_warrior_grid";
    public string UniqueId => Id;
    public string ListName => "Scenario Warrior (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ScenarioWarriorGridSelectorViewModel(container, context);
}

public class ScenarioPokemonSelectorEditorModule : IEditorModule
{
    public const string Id = "scenario_pokemon_selector";
    public string UniqueId => Id;
    public string ListName => "Scenario Pokemon";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ScenarioPokemonSelectorViewModel(container, context);
}

public class ScenarioAppearPokemonSelectorEditorModule : IEditorModule
{
    public const string Id = "scenario_appear_pokemon_selector";
    public string UniqueId => Id;
    public string ListName => "Scenario Appear Pokemon";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ScenarioAppearPokemonSelectorViewModel(container, context);
}

public class ScenarioKingdomSelectorEditorModule : IEditorModule
{
    public const string Id = "scenario_kingdom_selector";
    public string UniqueId => Id;
    public string ListName => "Scenario Kingdom";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ScenarioKingdomSelectorViewModel(container, context);
}

public class EventSpeakerSelectorEditorModule : IEditorModule
{
    public const string Id = "event_speaker_selector";
    public string UniqueId => Id;
    public string ListName => "Event Speaker";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new EventSpeakerSelectorViewModel(container, context);
}

public class ItemSelectorEditorModule : IEditorModule
{
    public const string Id = "item_selector";
    public string UniqueId => Id;
    public string ListName => "Item";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new ItemSelectorViewModel(container, context);
}

public class BuildingSelectorEditorModule : IEditorModule
{
    public const string Id = "building_selector";
    public string UniqueId => Id;
    public string ListName => "Building";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new BuildingSelectorViewModel(container, context);
}

public class BuildingGridEditorModule : IEditorModule
{
    public const string Id = "building_grid";
    public string UniqueId => Id;
    public string ListName => "Building (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new BuildingGridViewModel(container, context);
}

public class MsgGridEditorModule : IEditorModule
{
    public const string Id = "msg_grid";
    public string UniqueId => Id;
    public string ListName => "Text";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new MsgGridViewModel(container, context);
}

public class GimmickSelectorEditorModule : IEditorModule
{
    public const string Id = "gimmick_selector";
    public string UniqueId => Id;
    public string ListName => "Gimmick";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new GimmickSelectorViewModel(container, context);
}

public class GimmickGridEditorModule : IEditorModule
{
    public const string Id = "gimmick_grid";
    public string UniqueId => Id;
    public string ListName => "Gimmick (Grid)";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new GimmickGridViewModel(container, context);
}

public class KingdomSelectorEditorModule : IEditorModule
{
    public const string Id = "kingdom_selector";
    public string UniqueId => Id;
    public string ListName => "Kingdom";
    public ISaveableRefreshable NewViewModel(IServiceContainer container, IEditorContext context) => new KingdomSelectorViewModel(container, context);
}
