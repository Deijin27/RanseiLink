using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.Windows.ViewModels;

public class PopulateDefaultSpriteViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly ISettingService _settingService;
    private readonly RecentLoadRomSetting _recentLoadRomSetting;
    public PopulateDefaultSpriteViewModel(IDialogService dialogService, ISettingService settingService, IFileDropHandlerFactory fdhFactory)
    {
        _settingService = settingService;
        _recentLoadRomSetting = settingService.Get<RecentLoadRomSetting>();
        RomDropHandler = fdhFactory.NewRomDropHandler();
        File = _recentLoadRomSetting.Value;
        RomDropHandler.FileDropped += f =>
        {
            File = f;
        };

        FilePickerCommand = new RelayCommand(() =>
        {
            var file = dialogService.RequestRomFile();
            if (!string.IsNullOrEmpty(file))
            {
                File = file;
            }
        });
    }

    public ICommand FilePickerCommand { get; }

    public IFileDropHandler RomDropHandler { get; }

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


    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
        if (Result)
        {
            _recentLoadRomSetting.Value = File;
            _settingService.Save();
        }
    }
}

