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

    [StringOption("Randomization Seed", "Using the same seed will give the same result")]
    public string Seed { get; set; }





    [Header]
    public string TypesHeader => "Types";

    [BoolOption("Pokemon's Types")]
    public bool Types { get; set; }

    [BoolOption("Prevent duplicate types", "Prevent a pokemon's 1st and 2nd type being the same")]
    public bool PreventSameType { get; set; } = true;




    [Header]
    public string MovesHeader => "Moves";

    [BoolOption("Pokemon's Moves")]
    public bool Moves { get; set; }

    [BoolOption("Match move types to pokemon", "Ensure that randomly selected moves match the types of the pokemon (Note that STAB isn't a thing in this game so this doesn't affect power)")]
    public bool MatchMoveTypes { get; set; }

    [BoolOption("Avoid Dummy Moves", "Moves that are placeholders in the vanilla game will not be selected")]
    public bool AvoidDummyMoves { get; set; } = true;

    [BoolOption("Move Animations", "Ultimate chaos with random move animations")]
    public bool MoveAnimations { get; set; }





    [Header]
    public string Abilitieseader => "Abilities";

    [BoolOption("Pokemon's Abilities")]
    public bool Abilities { get; set; }

    [BoolOption("Avoid Dummy Abilities", "Abilities that are placeholders in the vanilla game will not be selected")]
    public bool AvoidDummyAbilities { get; set; } = true;





    [Header]
    public string StatsHeader => "Pokemon's Stats";

    [CollectionOption("Mode", new[] { StatRandomizationMode.None, StatRandomizationMode.Shuffle, StatRandomizationMode.Range })]
    public string StatRandomMode { get; set; } = StatRandomizationMode.None;

    [IntOption("Range Min", "Minimum value if range mode is selected")]
    public int StatRangeMin { get; set; } = 100;

    [IntOption("Range Max", "Maximum value if range mode is selected")]
    public int StatRangeMax { get; set; } = 300;





    [Header]
    public string ContHeader => "Warriors";

    [BoolOption("Warriors", "Randomize which warriors are where")]
    public bool Warriors { get; set; }

    [BoolOption("Warrior's Pokemon")]
    public bool ScenarioPokemon { get; set; }

    [BoolOption("Warrior's Skills")]
    public bool WarriorSkills { get; set; }

    [BoolOption("Avoid Dummy Warrior Skills", "Skills that are placeholders in the vanilla game will not be selected")]
    public bool AvoidDummyWarriorSkills { get; set; } = true;




    [Header]
    public string BattleHeader => "Battles";

    [BoolOption("Battle Maps", "Maps where battles take place are randomly selected. This excludes tutorials where it breaks scripted events.")]
    public bool BattleMaps { get; set; }

    [BoolOption("Disco Lights", "Randomly choose background colors for battles")]
    public bool DiscoLights { get; set; }

   

    

    

    [Header]
    public string ExtraHeader => "Other";

    [IntOption("Minimum max link value", "Each warrior has a max link with each pokemon; this setting prevents you from having a warrior who can't get stronger with the pokemon they've been randomly assigned. It defaults to 98 rather than 100 because the dialog that appears after battles with perfect links is annoying.", maximumValue: 100)]
    public int AllMaxLinkValue { get; set; } = 98;

    [BoolOption("Softlock Minimization", "Reduce the chance of softlocks caused by randomization (optimised for vanilla)")]
    public bool SoftlockMinimization { get; set; } = true;
}
