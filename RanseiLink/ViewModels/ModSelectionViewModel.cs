using RanseiLink.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;

namespace RanseiLink.ViewModels;

public class ModSelectionViewModel : ViewModelBase
{
    private readonly IServiceContainer _container;
    private readonly IModService _modService;
    private readonly IDialogService _dialogService;
    private readonly ModListItemViewModelFactory _itemViewModelFactory;
    private readonly IFallbackSpriteProvider _fallbackSpriteProvider;

    public ModSelectionViewModel(IServiceContainer container)
    {
        _container = container;
        _modService = container.Resolve<IModService>();
        _dialogService = container.Resolve<IDialogService>();
        _itemViewModelFactory = _container.Resolve<ModListItemViewModelFactory>();
        _fallbackSpriteProvider = _container.Resolve<IFallbackSpriteProvider>();

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

    private bool _outdatedModsExist;
    public bool OutdatedModsExist
    {
        get => _outdatedModsExist;
        set => RaiseAndSetIfChanged(ref _outdatedModsExist, value);
    }

    private readonly object _modItemsLock = new();
    public ObservableCollection<ModListItemViewModel> ModItems { get; } = new ObservableCollection<ModListItemViewModel>();

    public ICommand ModItemClicked { get; }

    public event Action<ModInfo> ModSelected;

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

    public ICommand CreateModCommand { get; }
    public ICommand ImportModCommand { get; }
    public ICommand UpgradeOutdatedModsCommand { get; }
    public ICommand PopulateGraphicsDefaultsCommand { get; }

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
