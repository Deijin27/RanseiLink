using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.DragDrop;
using RanseiLink.Settings;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModCreationViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly RecentLoadRomSetting _recentLoadRomSetting;
    private readonly ISettingService _settingService;
    public ModCreationViewModel(IDialogService dialogService, ISettingService settingService)
    {
        _settingService = settingService;
        _recentLoadRomSetting = settingService.Get<RecentLoadRomSetting>();
        ModInfo = new ModInfo();
        RomDropHandler = new RomDropHandler();
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

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
        if (result)
        {
            _recentLoadRomSetting.Value = File;
            _settingService.Save();
        }
        else
        {
            File = null;
        }
        
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
