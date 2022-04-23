using RanseiLink.PluginModule.Api;
using System.Collections.Generic;

namespace PartialTransferPlugin;

public class PartialTransferOptionForm : IPluginForm
{
    private const string Where = "Which mods to transfer between";
    private const string What = "What to transfer";

    public string Title => "Partial Transfer";
    public string ProceedButtonText => "Begin Transfer!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Transfer specific parts of a mod to another";

    public ICollection<string> Mods { get; set; }

    [CollectionOption("From", itemsSourcePropertyName: nameof(Mods), "The source of the transfer data", group: Where)]
    public string SourceMod { get; set; }

    [CollectionOption("To", itemsSourcePropertyName: nameof(Mods), "The source of the transfer data", group: Where)]
    public string DestinationMod { get; set; }


    [BoolOption("Abilities", group: What)]
    public bool Ability { get; set; }

    [BoolOption("Base Warriors", group: What)]
    public bool BaseWarrior { get; set; }

    [BoolOption("Battle Configs", group: What)]
    public bool BattleConfigs { get; set; }

    [BoolOption("Buildings", group: What)]
    public bool Building { get; set; }

    [BoolOption("Episodes", group: What)]
    public bool Episode { get; set; }

    [BoolOption("Event Speakers", group: What)]
    public bool EventSpeaker { get; set; }

    [BoolOption("Gimmicks", group: What)]
    public bool Gimmicks { get; set; }

    [BoolOption("Items", group: What)]
    public bool Item { get; set; }

    [BoolOption("Kingdoms", group: What)]
    public bool Kingdoms { get; set; }

    [BoolOption("Maps", group: What)]
    public bool Maps { get; set; }

    [BoolOption("Max Links", group: What)]
    public bool MaxLink { get; set; }

    [BoolOption("Move Ranges", group: What)]
    public bool MoveRange { get; set; }

    [BoolOption("Moves", group: What)]
    public bool Move { get; set; }

    [BoolOption("Pokemon", group: What)]
    public bool Pokemon { get; set; }

    [BoolOption("Scenario Appear Pokemon", group: What)]
    public bool ScenarioAppearPokemon { get; set; }

    [BoolOption("Scenario Kingdoms", group: What)]
    public bool ScenarioKingdom { get; set; }

    [BoolOption("Scenario Pokemon", group: What)]
    public bool ScenarioPokemon { get; set; }

    [BoolOption("Scenario Warriors", group: What)]
    public bool ScenarioWarrior { get; set; }

    [BoolOption("Sprites", group: What)]
    public bool Sprites { get; set; }

    [BoolOption("Text", group: What)]
    public bool Text { get; set; }

    [BoolOption("Warrior Skills", group: What)]
    public bool WarriorSkill { get; set; }
}
