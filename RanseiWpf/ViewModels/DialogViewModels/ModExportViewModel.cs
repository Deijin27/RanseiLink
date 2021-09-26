using Core.Services;
using RanseiWpf.DragDrop;
using System.Windows.Input;
using System;

namespace RanseiWpf.ViewModels
{
    public class ModExportViewModel : ViewModelBase
    {
        public ModExportViewModel(ModInfo modInfo, string initFile)
        {
            ModInfo = modInfo;
            RomDropHandler = new FolderDropHandler();
            RomDropHandler.FolderDropped += f =>
            {
                Folder = f;
                OkEnabled = true;
            };
            if (initFile != null)
            {
                Folder = initFile;
                OkEnabled = true;
            }

            DesktopCommand = new RelayCommand(() =>
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (System.IO.Directory.Exists(desktop))
                {
                    Folder = desktop;
                    OkEnabled = true;
                }
            });
        }

        public ICommand DesktopCommand { get; }

        public FolderDropHandler RomDropHandler { get; }

        private string _folder;
        public string Folder
        {
            get => _folder;
            set => RaiseAndSetIfChanged(ref _folder, value);
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
