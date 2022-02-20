using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using System.Windows.Input;
using RanseiLink.Core;

namespace RanseiLink.ViewModels;

public class ModExportViewModel : ViewModelBase
{
    public ModExportViewModel(ModInfo modInfo, string initFile)
    {
        ModInfo = modInfo;
        RomDropHandler = new FolderDropHandler();
        RomDropHandler.FolderDropped += f =>
        {
            Folder = f;
        };

        DesktopCommand = new RelayCommand(() =>
        {
            var desktop = FileUtil.DesktopDirectory;
            if (System.IO.Directory.Exists(desktop))
            {
                Folder = desktop;
            }
        });
    }

    public ICommand DesktopCommand { get; }

    public FolderDropHandler RomDropHandler { get; }

    private string _folder;
    public string Folder
    {
        get => _folder;
        set
        {
            if (RaiseAndSetIfChanged(ref _folder, value))
            {
                RaisePropertyChanged(nameof(OkEnabled));
            }
        }
    }

    public bool OkEnabled => _folder != null && System.IO.Directory.Exists(_folder);

    public ModInfo ModInfo { get; }

}
