using RanseiLink.Core;
using RanseiLink.PluginModule.Api;

namespace DisableEventsPlugin;
public static class ConstOptions
{
    public const string DisableEvents = "Disable Events";
    public const string EnableEvents = "Enable Events";
}

public class DisableEventsOptionForm : IPluginForm
{
    public string Title => "Disable Events Plugin";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [Text]
    public string Info => "Disable the game events. This will put you straight into a the overworld, disabling all story progression, cutscenes and tutorials. "
                        + "You only need to do this once to a particular rom, patches will not affect it.";

    [CollectionOption("Action", new[] { ConstOptions.DisableEvents, ConstOptions.EnableEvents })]
    public string Action { get; set; } = ConstOptions.DisableEvents;


}
