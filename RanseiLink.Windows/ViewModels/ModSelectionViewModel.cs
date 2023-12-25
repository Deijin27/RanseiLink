using RanseiLink.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;
using RanseiLink.Core.Settings;
using RanseiLink.Windows.Services;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.Windows.ViewModels;

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
    private readonly IFileDropHandlerFactory _fdhFactory;
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
    public ICommand ReportBugCommand { get; }


    public event Action<ModInfo> ModSelected;

    public ModSelectionViewModel(
        IModManager modManager,
        IDialogService dialogService,
        ISettingService settingService,
        ModListItemViewModelFactory modListItemViewModelFactory,
        IFallbackSpriteManager fallbackManager,
        IFileDropHandlerFactory fdhFactory)
    {
        _settingService = settingService;
        _modService = modManager;
        _dialogService = dialogService;
        _itemViewModelFactory = modListItemViewModelFactory;
        _fdhFactory = fdhFactory;
        ReportBugCommand = new RelayCommand(() => IssueReporter.ReportBug(App.Version));

        BindingOperations.EnableCollectionSynchronization(ModItems, _modItemsLock);

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
        var vm = new ModCreationViewModel(_dialogService, _settingService, _fdhFactory);
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
        var vm = new ModImportViewModel(_dialogService, _fdhFactory);
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
        var vm = new ModUpgradeViewModel(_dialogService, _settingService, _fdhFactory);
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
}
