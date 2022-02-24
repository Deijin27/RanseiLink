using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModCommitViewModel : ViewModelBase
{
    private string _file;
    private bool _includeSprites = true;

    public ModCommitViewModel(IDialogService dialogService, ModInfo modInfo, string initFile)
    {
        ModInfo = modInfo;
        RomDropHandler = new RomDropHandler();
        RomDropHandler.FileDropped += f =>
        {
            File = f;
        };

        File = initFile;

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

    public bool IncludeSprites
    {
        get => _includeSprites;
        set => RaiseAndSetIfChanged(ref _includeSprites, value);
    }

    public ModInfo ModInfo { get; }
}
