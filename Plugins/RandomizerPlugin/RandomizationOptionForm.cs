using RanseiLink.PluginModule.Api;
using System;

namespace RandomizerPlugin;

public static class StatRandomizationMode
{
    public const string None = "Don't randomize stats";
    public const string Shuffle = "Shuffle";
    public const string Range = "Within specified range";
}

public class RandomizationOptionForm : IPluginForm
{
    public string Title => "Configure Randomizer";
    public string ProceedButtonText => "Randomize!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Adjust the options to your choosing, then hit randomize to randomize the mod.";

    [StringOption("Randomization Seed", "Seed used for the randomizer. Using the same seed will give the same result")]
    public string Seed { get; set; } = Guid.NewGuid().ToString("N");

    [BoolOption("Warrior's Pokemon", "Randomize warrior's pokemon")]
    public bool ScenarioPokemon { get; set; }

    [BoolOption("Pokemon's Abilities", "Randomize pokemon's abilities")]
    public bool Abilities { get; set; }

    [BoolOption("Pokemon's Types", "Randomize pokemon's types")]
    public bool Types { get; set; }

    [BoolOption("Prevent duplicate types", "Prevent two of the same type on a pokemon")]
    public bool PreventSameType { get; set; } = true;

    [BoolOption("Pokemon's Moves", "Randomize pokemon's moves")]
    public bool Moves { get; set; }


    [CollectionOption("Mode", new[] { StatRandomizationMode.None, StatRandomizationMode.Shuffle, StatRandomizationMode.Range }, group: "Pokemon's Stats")]
    public string StatRandomMode { get; set; } = StatRandomizationMode.None;

    [IntOption("Range Min", "Minimum value if range mode is selected", group: "Pokemon's Stats")]
    public int StatRangeMin { get; set; } = 100;

    [IntOption("Range Max", "Maximum value if range mode is selected", group: "Pokemon's Stats")]
    public int StatRangeMax { get; set; } = 300;


    [BoolOption("Move Animations", "Ultimate chaos with random move animations")]
    public bool MoveAnimations { get; set; }

    [BoolOption("Warrior's Skills", "Randomize what skills the warriors have")]
    public bool WarriorSkills { get; set; }

    [BoolOption("Warriors", "Randomize which warriors are where")]
    public bool Warriors { get; set; }

    [BoolOption("Battle Maps", "Randomly choose a battle config for each kingdom and each battle building")]
    public bool BattleMaps { get; set; }

    [BoolOption("Disco Lights", "Randomly choose background colors for battles")]
    public bool DiscoLights { get; set; }

    [IntOption("Minimum max link value", "Set max link to at least this value", maximumValue: 100)]
    public int AllMaxLinkValue { get; set; } = 98;

    [BoolOption("Avoid Dummy Moves", "Avoid dummy moves when randomizing")]
    public bool AvoidDummyMoves { get; set; } = true;

    [BoolOption("Avoid Dummy Abilities", "Avoid dummy abilities when randomizing")]
    public bool AvoidDummyAbilities { get; set; } = true;

    [BoolOption("Avoid Dummy Warrior Skills", "Avoid dummy warrior skills when randomizing")]
    public bool AvoidDummyWarriorSkills { get; set; } = true;

    [BoolOption("Softlock Minimization", "Reduce the chance of softlocks caused by randomization (optimised for vanilla)")]
    public bool SoftlockMinimization { get; set; } = true;
}
