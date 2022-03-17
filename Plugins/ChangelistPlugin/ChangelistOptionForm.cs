using RanseiLink.PluginModule.Api;
using System.Collections.Generic;

namespace ChangelistPlugin;

public enum OutputType
{
    XML,
    TSV
}

public class ChangelistOptionForm : IPluginForm
{
    private const string WhichModsGroup = "Which mods to compare";
    private const string WhatChangelists = "What changelists";

    public string Title => "Changelist Generator";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Generate a list of what text has has changed by comparing to a mod that is unchanged. "
        + "TSV output type can be opened with google sheets, but xml is more readable if just reading the file in notepad. "
        + "To keep it more maintainable, while it supports most types and properties, it doesn't support everything.";

    public ICollection<string> Mods { get; set; }


    [CollectionOption("Unchanged Mod", itemsSourcePropertyName: nameof(Mods), group: WhichModsGroup)]
    public string UnchangedMod { get; set; }

    [CollectionOption("Changed Mod", itemsSourcePropertyName: nameof(Mods), group: WhichModsGroup)]
    public string ChangedMod { get; set; }



    [CollectionOption("Output Changelist As", new[] { OutputType.XML, OutputType.TSV })]
    public OutputType OutputType { get; set; } = OutputType.XML;



    [BoolOption("Abilities", group: WhatChangelists)]
    public bool Ability { get; set; }

    [BoolOption("Base Warriors", group: WhatChangelists)]
    public bool BaseWarrior { get; set; }

    [BoolOption("Battle Configs", group: WhatChangelists)]
    public bool BattleConfigs { get; set; }

    [BoolOption("Buildings", group: WhatChangelists)]
    public bool Building { get; set; }

    [BoolOption("Event Speakers", group: WhatChangelists)]
    public bool EventSpeaker { get; set; }

    [BoolOption("Gimmicks", group: WhatChangelists)]
    public bool Gimmicks { get; set; }

    [BoolOption("Items", group: WhatChangelists)]
    public bool Item { get; set; }

    [BoolOption("Kingdoms", group: WhatChangelists)]
    public bool Kingdoms { get; set; }

    [BoolOption("Moves", group: WhatChangelists)]
    public bool Move { get; set; }

    [BoolOption("Pokemon", group: WhatChangelists)]
    public bool Pokemon { get; set; }

    [BoolOption("Scenario Pokemon", group: WhatChangelists)]
    public bool ScenarioPokemon { get; set; }

    [BoolOption("Scenario Warriors", group: WhatChangelists)]
    public bool ScenarioWarrior { get; set; }

    [BoolOption("Text", group: WhatChangelists)]
    public bool Text { get; set; }

    [BoolOption("Warrior Skills", group: WhatChangelists)]
    public bool WarriorSkill { get; set; }

}
