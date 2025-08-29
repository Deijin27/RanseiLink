using RanseiLink.PluginModule.Api;
using System.Collections.Generic;

namespace PartialTransferPlugin;

public class PartialTransferOptionForm : IPluginForm
{
    public string Title => "Partial Transfer";
    public string ProceedButtonText => "Begin Transfer!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Transfer specific parts of a mod to another";

    public ICollection<string> Mods { get; set; }

    [Header]
    public string WhereHeader => "Which mods to transfer between";

    [CollectionOption("From", itemsSourcePropertyName: nameof(Mods))]
    public string SourceMod { get; set; }

    [CollectionOption("To", itemsSourcePropertyName: nameof(Mods))]
    public string DestinationMod { get; set; }

    [Header]
    public string Options => "Options";

    [BoolOption("Transfer Names", description: "When transferring things with names, e.g. Pokemon, uncheck this to copy everything except for the names")]
    public bool TransferNames { get; set; } = true;


    [Header]
    public string WhatHeader => "What to transfer";

    [BoolOption("Abilities")]
    public bool Ability { get; set; }

    [BoolOption("Banner")]
    public bool Banner { get; set; }

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

    [BoolOption("Maps")]
    public bool Maps { get; set; }

    [BoolOption("Max Links")]
    public bool MaxLink { get; set; }

    [BoolOption("Move Ranges")]
    public bool MoveRange { get; set; }

    [BoolOption("Moves")]
    public bool Move { get; set; }

    [BoolOption("Pokemon")]
    public bool Pokemon { get; set; }

    [BoolOption("Scenario Appear Pokemon")]
    public bool ScenarioAppearPokemon { get; set; }

    [BoolOption("Scenario Buildings")]
    public bool ScenarioBuilding { get; set; }

    [BoolOption("Scenario Warriors/Pokemon/Armies")]
    public bool ScenarioWarrior { get; set; }

    [BoolOption("Sprites")]
    public bool Sprites { get; set; }

    [BoolOption("Text")]
    public bool Text { get; set; }

    [BoolOption("Warrior Skills")]
    public bool WarriorSkill { get; set; }
}
