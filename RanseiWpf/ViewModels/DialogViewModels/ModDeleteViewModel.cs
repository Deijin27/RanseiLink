using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class ModDeleteViewModel : ViewModelBase
    {
        public ModDeleteViewModel(ModInfo modInfo)
        {
            ModInfo = modInfo;
        }

        public ModInfo ModInfo { get; }

    }
}
