using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModUpgradeViewModel : ViewModelBase
{
    public ModUpgradeViewModel(IDialogService dialogService, string initFile)
    {
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
}
