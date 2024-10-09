using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.GuiCore.Settings;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public interface IModSelectionViewModel
{
    event Action<ModInfo> ModSelected;
    ObservableCollection<IModListItemViewModel> ModItems { get; }
    List<IModListItemViewModel> AllItems { get; }
}

public class FilterableTag(string tag) : ViewModelBase
{
    private bool _checked;

    public string Tag { get; } = tag;
    public EventHandler? CheckedChanged;
    public bool Checked
    {
        get => _checked;
        set
        {
            if (SetProperty(ref _checked, value))
            {
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

public class ModSelectionViewModel : ViewModelBase, IModSelectionViewModel
{
    private readonly IModManager _modService;
    private readonly IAsyncDialogService _dialogService;
    private readonly ISettingService _settingService;
    private readonly ModListItemViewModelFactory _itemViewModelFactory;
    private readonly IFileDropHandlerFactory _fdhFactory;
    private readonly IDispatcherService _dispatcherService;
    private readonly PinnedModsSetting _pinnedModsSetting;
    private bool _outdatedModsExist;

    public bool OutdatedModsExist
    {
        get => _outdatedModsExist;
        set => SetProperty(ref _outdatedModsExist, value);
    }

    public ObservableCollection<FilterableTag> FilterableTags { get; } = [];
    public ObservableCollection<IModListItemViewModel> ModItems { get; } = [];
    public List<IModListItemViewModel> AllItems { get; } = [];
    public ICommand ModItemClicked { get; }
    public ICommand CreateModCommand { get; }
    public ICommand ImportModCommand { get; }
    public ICommand UpgradeOutdatedModsCommand { get; }
    public ICommand PopulateGraphicsDefaultsCommand { get; }
    public ICommand ReportBugCommand { get; }

    public ICommand ToggleThemeCommand { get; }
    public ICommand CrashCommand { get; }


    public event Action<ModInfo>? ModSelected;

    public ModSelectionViewModel(
        IModManager modManager,
        IAsyncDialogService dialogService,
        ISettingService settingService,
        ModListItemViewModelFactory modListItemViewModelFactory,
        IFallbackSpriteManager fallbackManager,
        IFileDropHandlerFactory fdhFactory,
        IDispatcherService dispatcherService,
        IThemeService themeService,
        IAppInfoService appInfoService)
    {
        _settingService = settingService;
        _modService = modManager;
        _dialogService = dialogService;
        _itemViewModelFactory = modListItemViewModelFactory;
        _fdhFactory = fdhFactory;
        _dispatcherService = dispatcherService;
        _pinnedModsSetting = _settingService.Get<PinnedModsSetting>();
        foreach (var item in _pinnedModsSetting.Value)
        {
            _pinnedModsFolders.Add(item);
        }
        ReportBugCommand = new RelayCommand(() => IssueReporter.ReportBug(appInfoService.Version));

        RefreshModItems();
        RefreshOutdatedModsExist();

        CreateModCommand = new RelayCommand(CreateMod);
        ImportModCommand = new RelayCommand(ImportMod);
        UpgradeOutdatedModsCommand = new RelayCommand(UpgradeOutdatedMods);
        PopulateGraphicsDefaultsCommand = new RelayCommand(fallbackManager.PopulateGraphicsDefaults);

        ModItemClicked = new RelayCommand<ModInfo>(mi =>
        {
            if (mi != null) ModSelected?.Invoke(mi);
        });

        ToggleThemeCommand = new RelayCommand(themeService.ToggleTheme);
        CrashCommand = new RelayCommand(() => throw new Exception("Alert! Alert! Intentional Crash Detected!"));
    }

    private void ReloadTags()
    {
        foreach (var tag in FilterableTags)
        {
            tag.CheckedChanged -= TagCheckedChanged;
        }
        FilterableTags.Clear();
        var tags = AllItems.SelectMany(x => x.Mod.Tags).Distinct().ToList();
        tags.Sort();
        foreach ( var tag in tags)
        {
            var ft = new FilterableTag(tag);
            FilterableTags.Add(ft);
            ft.CheckedChanged += TagCheckedChanged;
        }
    }

    private void TagCheckedChanged(object? sender, EventArgs e)
    {
        // apply filtering
        var checkedTags = FilterableTags.Where(x => x.Checked).ToList();
        ModItems.Clear();
        if (checkedTags.Count != 0)
        {
            foreach (var item in AllItems)
            {
                if (checkedTags.All(tag => item.Mod.Tags.Contains(tag.Tag)))
                {
                    ModItems.Add(item);
                }
            }
        }
        else
        {
            foreach (var item in AllItems)
            {
                ModItems.Add(item);
            }
        }
    }

    private void RefreshOutdatedModsExist()
    {
        OutdatedModsExist = _modService.GetModInfoPreviousVersions().Any();
    }

    /// <summary>
    /// The folder paths of newly created mods
    /// </summary>
    private readonly HashSet<string> _newModsFolders = new(StringComparer.OrdinalIgnoreCase);


    /// <summary>
    /// The folder paths of pinned mods
    /// </summary>
    private readonly HashSet<string> _pinnedModsFolders = new(StringComparer.OrdinalIgnoreCase);

    public void RefreshModItems()
    {
        var modListItems = new List<IModListItemViewModel>();
        foreach (var mi in _modService.GetAllModInfo())
        {
            var item = _itemViewModelFactory(mi, GetKnownTags);
            modListItems.Add(item);

            item.IsPinned = _pinnedModsFolders.Contains(mi.FolderPath);
            item.IsNew = _newModsFolders.Contains(mi.FolderPath);
            item.RequestRefresh += RefreshModItems;
            item.RequestRemove += RemoveItem;
            item.IsPinnedChanged += Mod_IsPinnedChanged;
        }

        var sortedMods = modListItems
            .OrderBy(x => !x.IsPinned)
            .ThenBy(x => !x.IsNew)
            .ThenBy(i => i.Mod.Name);

        // If necessary, I could do the stuff before showing the dialog for the fast things, then not have to do this locking
        _dispatcherService.Invoke(() =>
        {
            ModItems.Clear();
            AllItems.Clear();

            foreach (var item in sortedMods)
            {
                ModItems.Add(item);
                AllItems.Add(item);
            }
            ReloadTags();
        });
    }

    private void Mod_IsPinnedChanged(IModListItemViewModel mod)
    {
        _pinnedModsFolders.Clear();
        foreach (var item in AllItems.Where(x => x.IsPinned))
        {
            _pinnedModsFolders.Add(item.Mod.FolderPath);
        }
        _pinnedModsSetting.Value = _pinnedModsFolders.ToArray();
        _settingService.Save();
        RefreshModItems();
    }

    private List<string> GetKnownTags()
    {
        return FilterableTags.Select(x => x.Tag).ToList();
    }

    private void RemoveItem(IModListItemViewModel mod)
    {
        _dispatcherService.Invoke(() =>
        {
            AllItems.Remove(mod);
            ModItems.Remove(mod);
            ReloadTags();
        });
    }

    private async Task CreateMod()
    {
        var vm = new ModCreationViewModel(_dialogService, _settingService, _fdhFactory, GetKnownTags());
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
                newMod = _modService.Create(vm.File, vm.Metadata);
                _newModsFolders.Add(newMod.FolderPath);
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
        var vm = new ModImportViewModel(_dialogService, _fdhFactory);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Importing mod..."));
            try
            {
                var importedMod = _modService.Import(vm.File);
                _newModsFolders.Add(importedMod.FolderPath);

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
        var vm = new ModUpgradeViewModel(_dialogService, _settingService, _fdhFactory);
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

