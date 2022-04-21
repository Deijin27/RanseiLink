using RanseiLink.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;

namespace RanseiLink.ViewModels;

public interface IModSelectionViewModel
{
    ICommand CreateModCommand { get; }
    ICommand ImportModCommand { get; }
    ICommand ModItemClicked { get; }
    ObservableCollection<IModListItemViewModel> ModItems { get; }
    bool OutdatedModsExist { get; set; }
    ICommand PopulateGraphicsDefaultsCommand { get; }
    ICommand UpgradeOutdatedModsCommand { get; }

    event Action<ModInfo> ModSelected;

    void RefreshModItems();
}

public class ModSelectionViewModel : ViewModelBase, IModSelectionViewModel
{
    private readonly IModManager _modService;
    private readonly IDialogService _dialogService;
    private readonly IModListItemViewModelFactory _itemViewModelFactory;
    private readonly IFallbackSpriteProvider _fallbackSpriteProvider;
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
        IModListItemViewModelFactory modListItemViewModelFactory,
        IFallbackSpriteProvider fallbackSpriteProvider)
    {
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
                ModItems.Add(_itemViewModelFactory.CreateViewModel(this, mi));
            }
        }
    }

    private void CreateMod()
    {
        if (_dialogService.CreateMod(out ModInfo modInfo, out string romPath))
        {
            _dialogService.ProgressDialog(progress =>
            {
                progress.Report(new ProgressInfo("Creating mod..."));
                ModInfo newMod;
                try
                {
                    newMod = _modService.Create(romPath, modInfo.Name, modInfo.Version, modInfo.Author);
                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
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
    }
    private void ImportMod()
    {
        if (_dialogService.ImportMod(out string file))
        {
            _dialogService.ProgressDialog(progress =>
            {
                progress.Report(new ProgressInfo("Importing mod..."));
                try
                {
                    _modService.Import(file);

                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
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
    }

    private void UpgradeOutdatedMods()
    {
        if (_dialogService.UpgradeMods(out string romPath))
        {
            _dialogService.ProgressDialog(progress =>
            {
                progress.Report(new ProgressInfo("Upgrading mods..."));
                try
                {
                    _modService.UpgradeModsToLatestVersion(_modService.GetModInfoPreviousVersions(), romPath);
                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
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

    private void PopulateGraphicsDefaults()
    {
        if (_dialogService.PopulateDefaultSprites(out string romPath))
        {
            _dialogService.ProgressDialog(progress =>
            {
                try
                {
                    _fallbackSpriteProvider.Populate(romPath, progress);
                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                            title: "Error Populating Default Sprites",
                            message: e.ToString(),
                            type: MessageBoxType.Error
                        ));
                    return;
                }
            });
        }
    }
}
