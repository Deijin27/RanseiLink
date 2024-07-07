using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.GuiCore.ViewModels;

public class ModCreationViewModel : ModMetadataViewModelBase, IModalDialogViewModel<bool>
{
    private readonly RecentLoadRomSetting _recentLoadRomSetting;
    private readonly ISettingService _settingService;
    public ModCreationViewModel(IAsyncDialogService dialogService, ISettingService settingService, IFileDropHandlerFactory fdhFactory, List<string> knownTags)
        :base(new(), knownTags)
    {
        _settingService = settingService;
        _recentLoadRomSetting = settingService.Get<RecentLoadRomSetting>();
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
        OnClosing();
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
            if (SetProperty(ref _file, value))
            {
                RaisePropertyChanged(nameof(OkEnabled));
            }
        }
    }

    public bool OkEnabled => _file != null && System.IO.File.Exists(_file);

    
}
