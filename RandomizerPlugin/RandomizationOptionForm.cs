using RanseiLink.PluginModule.Api;
using System;

namespace RandomizerPlugin;

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

    [UIntOption("Minimum max link value", "Set max link to at least this value", maximumValue: 100)]
    public uint AllMaxLinkValue { get; set; } = 98;

    [BoolOption("Avoid Dummys", "Avoid dummy moves, abilities, etc when randomizing")]
    public bool AvoidDummys { get; set; } = true;
}
