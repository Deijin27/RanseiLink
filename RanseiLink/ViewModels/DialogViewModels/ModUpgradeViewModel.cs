using RanseiLink.Core.Services;
using RanseiLink.DragDrop;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModUpgradeViewModel : ViewModelBase
{
    public ModUpgradeViewModel(IDialogService dialogService, string initFile)
    {
        RomDropHandler = new RomDropHandler();
        File = initFile;
        if (initFile != null)
        {
            OkEnabled = true;
        }
        RomDropHandler.FileDropped += f =>
        {
            File = f;
            OkEnabled = true;
        };

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
}
