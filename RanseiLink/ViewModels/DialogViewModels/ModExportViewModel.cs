using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModExportViewModel : ViewModelBase
{
    public ModExportViewModel(IDialogService dialogService, ModInfo modInfo, string initFolder)
    {
        Folder = initFolder;
        ModInfo = modInfo;
        RomDropHandler = new FolderDropHandler();
        RomDropHandler.FolderDropped += f =>
        {
            Folder = f;
        };

        FolderPickerCommand = new RelayCommand(() =>
        {
            if (dialogService.RequestFolder("Select a folder to export the mod into", out string folder))
            {
                Folder = folder;
            }
        });
    }

    public ICommand FolderPickerCommand { get; }

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
