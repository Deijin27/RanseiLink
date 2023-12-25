using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.GuiCore.ViewModels;

public class ModCreationViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly RecentLoadRomSetting _recentLoadRomSetting;
    private readonly ISettingService _settingService;
    public ModCreationViewModel(IAsyncDialogService dialogService, ISettingService settingService, IFileDropHandlerFactory fdhFactory)
    {
        _settingService = settingService;
        _recentLoadRomSetting = settingService.Get<RecentLoadRomSetting>();
        ModInfo = new ModInfo();
        RomDropHandler = fdhFactory.NewRomDropHandler();
        _file = _recentLoadRomSetting.Value;
        RomDropHandler.FileDropped += f =>
        {
            File = f;
        };

        FilePickerCommand = new RelayCommand(async () =>
        {
            var file = await dialogService.RequestRomFile();
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
            _recentLoadRomSetting.Value = File!;
            _settingService.Save();
        }
        else
        {
            File = string.Empty;
        }
        
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

    public ModInfo ModInfo { get; }

    public string? Name
    {
        get => ModInfo.Name;
        set => RaiseAndSetIfChanged(ModInfo.Name, value, v => ModInfo.Name = v);
    }

    public string? Author
    {
        get => ModInfo.Author;
        set => RaiseAndSetIfChanged(ModInfo.Author, value, v => ModInfo.Author = v);
    }

    public string? Version
    {
        get => ModInfo.Version;
        set => RaiseAndSetIfChanged(ModInfo.Version, value, v => ModInfo.Version = v);
    }
}
