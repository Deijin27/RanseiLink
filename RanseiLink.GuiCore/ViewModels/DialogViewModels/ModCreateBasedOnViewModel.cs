using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class ModCreateBasedOnViewModel : ModMetadataViewModelBase, IModalDialogViewModel<bool>
{
    public ModCreateBasedOnViewModel(ModInfo baseMod, List<string> knownTags) : base(baseMod.CopyMetadata(), knownTags)
    {
        BaseMod = baseMod;
    }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        OnClosing();
        Result = result;
    }

    public ModInfo BaseMod { get; }
}
