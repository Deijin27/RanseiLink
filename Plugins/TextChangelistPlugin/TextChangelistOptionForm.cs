using RanseiLink.PluginModule.Api;
using System.Collections.Generic;

namespace TextChangelistPlugin;

public class TextChangelistOptionForm : IPluginForm
{
    public string Title => "Text Changelist Generator";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Description => "Generate a list of what text has has changed by comparing to a mod that is unchanged";

    public ICollection<string> Mods { get; set; }

    [CollectionOption("Unchanged Mod", itemsSourcePropertyName: nameof(Mods))]
    public string UnchangedMod { get; set; }

    [CollectionOption("Changed Mod", itemsSourcePropertyName: nameof(Mods))]
    public string ChangedMod { get; set; }

    [BoolOption("Include Text")]
    public bool IncludeText { get; set; } = true;

    [BoolOption("Include Box Config")]
    public bool IncludeBoxConfig { get; set; } = true;

    [BoolOption("Include Box Context")]
    public bool IncludeContext { get; set; } = true;

}
