using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class ModDeleteViewModel : ViewModelBase
{
    public ModDeleteViewModel(ModInfo modInfo)
    {
        ModInfo = modInfo;
    }

    public ModInfo ModInfo { get; }

}
