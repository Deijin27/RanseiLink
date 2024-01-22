using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class ModEditInfoViewModel : ModMetadataViewModelBase, IModalDialogViewModel<bool>
{
    public ModEditInfoViewModel(ModInfo info, List<string> knownTags) : base(info.CopyMetadata(), knownTags)
    {
        ModInfo = info;
    }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        OnClosing();
        Result = result;
        if (result)
        {
            ModInfo.LoadMetadata(Metadata);
        }
    }

    public ModInfo ModInfo { get; private set; }
    
}
