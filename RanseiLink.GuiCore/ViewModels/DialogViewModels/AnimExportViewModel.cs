using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.GuiCore.ViewModels;

public class AnimExportViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly ISettingService _settingService;
    private readonly RecentExportAnimFolderSetting _recentExportModFolderSetting;
    public AnimExportViewModel(IAsyncDialogService dialogService, ISettingService settingService, ModInfo modInfo,
        RLAnimationFormat[] selectableFormats, RLAnimationFormat initialSelectedFormat,
        IFolderDropHandler folderDropHandler)
    {
        ExportFormats = selectableFormats;
        _selectedFormat = initialSelectedFormat;
        _settingService = settingService;
        _recentExportModFolderSetting = settingService.Get<RecentExportAnimFolderSetting>();
        _folder = _recentExportModFolderSetting.Value;
        ModInfo = modInfo;
        FolderDropHandler = folderDropHandler;
        FolderDropHandler.FolderDropped += f =>
        {
            Folder = f;
        };

        FolderPickerCommand = new RelayCommand(async () =>
        {
            var folder = await dialogService.ShowOpenFolderDialog(new OpenFolderDialogSettings { Title = "Select a folder to export the animation to" });
            if (!string.IsNullOrEmpty(folder))
            {
                Folder = folder;
            }
        });
    }


    public RLAnimationFormat[] ExportFormats { get; }

    public RLAnimationFormat SelectedFormat
    {
        get => _selectedFormat;
        set => RaiseAndSetIfChanged(ref  _selectedFormat, value);
    }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        Result = result;
        if (result)
        {
            _recentExportModFolderSetting.Value = Folder;
            _settingService.Save();
        }
    }

    public ICommand FolderPickerCommand { get; }

    public IFolderDropHandler FolderDropHandler { get; }

    private string _folder;
    private RLAnimationFormat _selectedFormat;

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

    public bool OkEnabled => _folder != null && Directory.Exists(_folder);

    public ModInfo ModInfo { get; }

}
