using Core.Services;
using RanseiWpf.DragDrop;

namespace RanseiWpf.ViewModels
{
    public class ModCommitViewModel : ViewModelBase
    {
        public ModCommitViewModel(ModInfo modInfo, string initFile)
        {
            ModInfo = modInfo;
            RomDropHandler = new RomDropHandler();
            RomDropHandler.FileDropped += f =>
            {
                File = f;
                OkEnabled = true;
            };
            if (initFile != null)
            {
                File = initFile;
                OkEnabled = true;
            }
        }

        public RomDropHandler RomDropHandler { get; }

        private string _file;
        public string File
        {
            get => _file;
            set => RaiseAndSetIfChanged(ref _file, value);
        }

        private bool _okEnabled = false;
        public bool OkEnabled
        {
            get => _okEnabled;
            set => RaiseAndSetIfChanged(ref _okEnabled, value);
        }

        public ModInfo ModInfo { get; }
    }
}
