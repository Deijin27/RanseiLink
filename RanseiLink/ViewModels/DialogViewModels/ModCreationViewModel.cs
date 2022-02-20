using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModCreationViewModel : ViewModelBase
{
    public ModCreationViewModel(IDialogService dialogService, string initFile)
    {
        ModInfo = new ModInfo();
        RomDropHandler = new RomDropHandler();
        File = initFile;
        RomDropHandler.FileDropped += f =>
        {
            File = f;
        };

        FilePickerCommand = new RelayCommand(() =>
        {
            if (dialogService.RequestRomFile(out string file))
            {
                File = file;
            }
        });
    }

    public ICommand FilePickerCommand { get; }

    public RomDropHandler RomDropHandler { get; }

    private string _file;
    public string File
    {
        get => _file;
        set
        {
            if (RaiseAndSetIfChanged(ref _file, value))
            {
                RaisePropertyChanged(nameof(OkEnabled));
            }
        }
    }

    public bool OkEnabled => _file != null && System.IO.File.Exists(_file);

    public ModInfo ModInfo { get; }

    public string Name
    {
        get => ModInfo.Name;
        set => RaiseAndSetIfChanged(ModInfo.Name, value, v => ModInfo.Name = v);
    }

    public string Author
    {
        get => ModInfo.Author;
        set => RaiseAndSetIfChanged(ModInfo.Author, value, v => ModInfo.Author = v);
    }

    public string Version
    {
        get => ModInfo.Version;
        set => RaiseAndSetIfChanged(ModInfo.Version, value, v => ModInfo.Version = v);
    }
}
