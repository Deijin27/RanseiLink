using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using System.Windows.Input;

namespace RanseiLink.XP.ViewModels;

public class ModExportViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly ISettingService _settingService;
    private readonly RecentExportModFolderSetting _recentExportModFolderSetting;
    public ModExportViewModel(IAsyncDialogService dialogService, ISettingService settingService, ModInfo modInfo, IFolderDropHandler folderDropHandler)
    {
        _settingService = settingService;
        _recentExportModFolderSetting = settingService.Get<RecentExportModFolderSetting>();
        Folder = _recentExportModFolderSetting.Value;
        ModInfo = modInfo;
        FolderDropHandler = folderDropHandler;
        FolderDropHandler.FolderDropped += f =>
        {
            Folder = f;
        };

        FolderPickerCommand = new RelayCommand(async () =>
        {
            var folder = await dialogService.ShowOpenFolderDialog(new OpenFolderDialogSettings { Title = "Select a folder to export the mod into" });
            if (!string.IsNullOrEmpty(folder))
            {
                Folder = folder;
            }
        });
    }

    public bool Result { get; private set; }

    public void OnClosing(bool result)
    {
        Result = result;
        if (result)
        {
            _recentExportModFolderSetting.Value = Folder!;
            _settingService.Save();
        }
    }

    public ICommand FolderPickerCommand { get; }

    public IFolderDropHandler FolderDropHandler { get; }

    private string _folder;
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

    public bool OkEnabled => _folder != null && System.IO.Directory.Exists(_folder);

    public ModInfo ModInfo { get; }

}
