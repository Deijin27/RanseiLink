using RanseiLink.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;
using RanseiLink.Core.Settings;

namespace RanseiLink.ViewModels;

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
    private readonly IFallbackDataProvider _fallbackSpriteProvider;
    private readonly object _modItemsLock = new();
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

    public event Action<ModInfo> ModSelected;

    public ModSelectionViewModel(
        IModManager modManager,
        IDialogService dialogService,
        ISettingService settingService,
        ModListItemViewModelFactory modListItemViewModelFactory,
        IFallbackDataProvider fallbackSpriteProvider)
    {
        _settingService = settingService;
        _modService = modManager;
        _dialogService = dialogService;
        _itemViewModelFactory = modListItemViewModelFactory;
        _fallbackSpriteProvider = fallbackSpriteProvider;

        BindingOperations.EnableCollectionSynchronization(ModItems, _modItemsLock);

        RefreshModItems();
        RefreshOutdatedModsExist();

        CreateModCommand = new RelayCommand(CreateMod);
        ImportModCommand = new RelayCommand(ImportMod);
        UpgradeOutdatedModsCommand = new RelayCommand(UpgradeOutdatedMods);
        PopulateGraphicsDefaultsCommand = new RelayCommand(PopulateGraphicsDefaults);

        ModItemClicked = new RelayCommand<ModInfo>(mi =>
        {
            ModSelected?.Invoke(mi);
        });
    }

    private void RefreshOutdatedModsExist()
    {
        OutdatedModsExist = _modService.GetModInfoPreviousVersions().Any();
    }

    public void RefreshModItems()
    {
        lock (_modItemsLock)
        {
            ModItems.Clear();
            foreach (var mi in _modService.GetAllModInfo().OrderBy(i => i.Name))
            {
                ModItems.Add(_itemViewModelFactory(this, mi));
            }
        }
    }

    private void CreateMod()
    {
        var vm = new ModCreationViewModel(_dialogService, _settingService);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        _dialogService.ProgressDialog(progress =>
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
    private void ImportMod()
    {
        var vm = new ModImportViewModel(_dialogService);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        _dialogService.ProgressDialog(progress =>
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

    private void UpgradeOutdatedMods()
    {
        var vm = new ModUpgradeViewModel(_dialogService, _settingService);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        _dialogService.ProgressDialog(progress =>
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

    private void PopulateGraphicsDefaults()
    {
        var vm = new PopulateDefaultSpriteViewModel(_dialogService, _settingService);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception error = null;
        PopulateResult result = null;
        _dialogService.ProgressDialog(progress =>
        {
            try
            {
                result = _fallbackSpriteProvider.Populate(vm.File, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Populating Default Sprites",
                message: error.ToString(),
                type: MessageBoxType.Error
                ));
        }
        else if (!result.Success)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Failed to Populate Default Sprites",
                message: result.FailureReason,
                type: MessageBoxType.Error
                ));
        }
    }
}
