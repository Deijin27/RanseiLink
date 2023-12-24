using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.Windows.Settings;
using System.Windows.Input;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.Windows.ViewModels;

public class ModCommitViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private string _file;
    private bool _includeSprites = true;
    private readonly RecentCommitRomSetting _recentCommitRomSetting;
    private readonly PatchSpritesSetting _patchSpritesSetting;
    private readonly ISettingService _settingService;

    public ModCommitViewModel(IDialogService dialogService, ISettingService settingService, ModInfo modInfo, IFileDropHandlerFactory fdhFactory)
    {
        _settingService = settingService;
        _recentCommitRomSetting = settingService.Get<RecentCommitRomSetting>();
        _patchSpritesSetting = settingService.Get<PatchSpritesSetting>();
        ModInfo = modInfo;
        RomDropHandler = fdhFactory.NewRomDropHandler();
        RomDropHandler.FileDropped += f =>
        {
            File = f;
        };

        File = _recentCommitRomSetting.Value;
        _includeSprites = _patchSpritesSetting.Value;

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
    public PatchOptions PatchOpt { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;

        if (result)
        {
            _recentCommitRomSetting.Value = File;
            _patchSpritesSetting.Value = IncludeSprites;
            _settingService.Save();
            
            if (IncludeSprites)
            {
                PatchOpt |= PatchOptions.IncludeSprites;
            }
        }
    }

    public ICommand FilePickerCommand { get; }

    public IFileDropHandler RomDropHandler { get; }

    
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
