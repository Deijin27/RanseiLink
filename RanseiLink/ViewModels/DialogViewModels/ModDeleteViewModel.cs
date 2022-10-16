using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class ModDeleteViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    public ModDeleteViewModel(ModInfo modInfo)
    {
        ModInfo = modInfo;
    }

    public ModInfo ModInfo { get; }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        Result = result;
    }
}
