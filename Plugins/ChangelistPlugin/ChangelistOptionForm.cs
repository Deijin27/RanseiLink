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
    public string Title => "Changelist Generator";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Generate a list of what text has has changed by comparing to a mod that is unchanged. "
        + "TSV output type can be opened with google sheets, but xml is more readable if just reading the file in notepad. "
        + "To keep it more maintainable, while it supports most types and properties, it doesn't support everything.";

    public ICollection<string> Mods { get; set; }

    [Header]
    public string OutputHeader => "Output";

    [CollectionOption("Output Changelist As", new[] { OutputType.XML, OutputType.TSV })]
    public OutputType OutputType { get; set; } = OutputType.XML;

    [Header]
    public string WhichModsHeader => "Which mods to compare";

    [CollectionOption("Unchanged Mod", itemsSourcePropertyName: nameof(Mods))]
    public string UnchangedMod { get; set; }

    [CollectionOption("Changed Mod", itemsSourcePropertyName: nameof(Mods))]
    public string ChangedMod { get; set; }


    [Header]
    public string WhatChangelistsHeader => "What changelists";

    [BoolOption("Abilities")]
    public bool Ability { get; set; }

    [BoolOption("Base Warriors")]
    public bool BaseWarrior { get; set; }

    [BoolOption("Battle Configs")]
    public bool BattleConfigs { get; set; }

    [BoolOption("Buildings")]
    public bool Building { get; set; }

    [BoolOption("Episodes")]
    public bool Episode { get; set; }

    [BoolOption("Event Speakers")]
    public bool EventSpeaker { get; set; }

    [BoolOption("Gimmicks")]
    public bool Gimmicks { get; set; }

    [BoolOption("Gimmick Ranges")]
    public bool GimmickRange { get; set; }

    [BoolOption("Items")]
    public bool Item { get; set; }

    [BoolOption("Kingdoms")]
    public bool Kingdoms { get; set; }

    [BoolOption("Moves")]
    public bool Move { get; set; }

    [BoolOption("Move Ranges")]
    public bool MoveRange { get; set; }

    [BoolOption("Pokemon")]
    public bool Pokemon { get; set; }

    [BoolOption("Scenario Pokemon")]
    public bool ScenarioPokemon { get; set; }

    [BoolOption("Scenario Warriors")]
    public bool ScenarioWarrior { get; set; }

    [BoolOption("Scenario Buildings")]
    public bool ScenarioBuilding { get; set; }

    [BoolOption("Text")]
    public bool Text { get; set; }

    [BoolOption("Warrior Skills")]
    public bool WarriorSkill { get; set; }

}
