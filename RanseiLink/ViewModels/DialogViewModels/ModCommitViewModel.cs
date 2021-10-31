using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels
{
    public class ModCommitViewModel : ViewModelBase
    {
        public ModCommitViewModel(IDialogService dialogService, ModInfo modInfo, string initFile)
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

            FilePickerCommand = new RelayCommand(() =>
            {
                if (dialogService.RequestRomFile(out string file))
                {
                    File = file;
                    OkEnabled = true;
                }
            });
        }

        public ICommand FilePickerCommand { get; }

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
