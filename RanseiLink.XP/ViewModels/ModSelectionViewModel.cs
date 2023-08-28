using Avalonia.Threading;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.XP.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiLink.XP.ViewModels;


public interface IModSelectionViewModel
{
    event Action<ModInfo> ModSelected;
    ObservableCollection<IModListItemViewModel> ModItems { get; }
    void RefreshModItems();
}
public class ModSelectionViewModel : ViewModelBase, IModSelectionViewModel
{
    private readonly IModManager _modService;
    private readonly IDialogService _dialogService;
    private readonly ISettingService _settingService;
    private readonly ModListItemViewModelFactory _itemViewModelFactory;
    private bool _outdatedModsExist;

    public bool OutdatedModsExist
    {
        get => _outdatedModsExist;
        set => RaiseAndSetIfChanged(ref _outdatedModsExist, value);
    }

    public ObservableCollection<IModListItemViewModel> ModItems { get; } = new ObservableCollection<IModListItemViewModel>();

    public ICommand ModItemClicked { get; }
    public ICommand CreateModCommand { get; }
    public ICommand ImportModCommand { get; }
    public ICommand UpgradeOutdatedModsCommand { get; }
    public ICommand PopulateGraphicsDefaultsCommand { get; }
    public ICommand ReportBugCommand { get; }

    public ICommand ToggleThemeCommand { get; }
    public ICommand CrashCommand { get; }

    public event Action<ModInfo> ModSelected;

    public ModSelectionViewModel(
        IModManager modManager,
        IDialogService dialogService,
        ISettingService settingService,
        ModListItemViewModelFactory modListItemViewModelFactory,
        IFallbackSpriteManager fallbackManager,
        IThemeService themeService)
    {
        _settingService = settingService;
        _modService = modManager;
        _dialogService = dialogService;
        _itemViewModelFactory = modListItemViewModelFactory;
        ReportBugCommand = new RelayCommand(IssueReporter.ReportBug);


        RefreshModItems();
        RefreshOutdatedModsExist();

        CreateModCommand = new RelayCommand(CreateMod);
        ImportModCommand = new RelayCommand(ImportMod);
        UpgradeOutdatedModsCommand = new RelayCommand(UpgradeOutdatedMods);
        PopulateGraphicsDefaultsCommand = new RelayCommand(fallbackManager.PopulateGraphicsDefaults);

        ModItemClicked = new RelayCommand<ModInfo>(mi =>
        {
            ModSelected?.Invoke(mi);
        });
        ToggleThemeCommand = new RelayCommand(themeService.ToggleTheme);
        CrashCommand = new RelayCommand(() => throw new Exception("Alert! Alert! Intentional Crash Detected!"));
    }

    private void RefreshOutdatedModsExist()
    {
        OutdatedModsExist = _modService.GetModInfoPreviousVersions().Any();
    }

    public void RefreshModItems()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            ModItems.Clear();
            foreach (var mi in _modService.GetAllModInfo().OrderBy(i => i.Name))
            {
                ModItems.Add(_itemViewModelFactory(this, mi));
            }
        });
    }

    private async Task CreateMod()
    {
        var vm = new ModCreationViewModel(_dialogService, _settingService);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Creating mod..."));
            ModInfo newMod;
            try
            {
                newMod = _modService.Create(vm.File, vm.ModInfo.Name, vm.ModInfo.Version, vm.ModInfo.Author);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Creating Mod",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }

            RefreshModItems();
            progress.Report(new ProgressInfo("Mod created successfully!", 100));
        });
    }
    private async Task ImportMod()
    {
        var vm = new ModImportViewModel(_dialogService);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Importing mod..."));
            try
            {
                _modService.Import(vm.File);

            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Importing Mod",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }
            RefreshModItems();
            RefreshOutdatedModsExist();
            progress.Report(new ProgressInfo("Done!", 100));
        });
    }

    private async Task UpgradeOutdatedMods()
    {
        var vm = new ModUpgradeViewModel(_dialogService, _settingService);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Upgrading mods..."));
            try
            {
                _modService.UpgradeModsToLatestVersion(_modService.GetModInfoPreviousVersions(), vm.File);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                        title: "Error Upgrading Mods",
                        message: e.Message,
                        type: MessageBoxType.Error
                    ));
                return;
            }
            RefreshModItems();
            RefreshOutdatedModsExist();
            progress.Report(new ProgressInfo("Done!", 100));
        });
    }
}