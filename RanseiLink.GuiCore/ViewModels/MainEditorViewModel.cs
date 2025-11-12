using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RanseiLink.GuiCore.ViewModels;

public record EditorModuleListItem(string DisplayName, string ModuleId);

public interface IMainEditorViewModel
{
    Task SetMod(ModInfo mod);
    void Deactivate();
    string? CurrentModuleId { get; }
    bool TryGetModule(string moduleId, [NotNullWhen(true)] out EditorModule? module);
    void NavigateTo(string? moduleId, int? selectId = null);
}
public class MainEditorViewModel : ViewModelBase, IMainEditorViewModel
{
    private readonly IAsyncDialogService _dialogService;
    private readonly IModPatchingService _modPatcher;
    private readonly ISettingService _settingService;
    private readonly IModServiceGetterFactory _modKernelFactory;
    private readonly IFileDropHandlerFactory _fdhFactory;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly EditorModuleOrderSetting _editorModuleOrderSetting;
    private ModInfo? _mod;
    private IServiceGetter? _modServiceGetter;
    private EditorModule? _currentModule;
    private IBannerService? _bannerService;

    public MainEditorViewModel(
        IAsyncDialogService dialogService,
        IModPatchingService modPatcher,
        ISettingService settingService,
        IPluginLoader pluginLoader,
        IModServiceGetterFactory modKernelFactory,
        IEnumerable<EditorModule> modules,
        IFileDropHandlerFactory fdhFactory,
        IPathToImageConverter pathToImageConverter)
    {
        _modKernelFactory = modKernelFactory;
        _fdhFactory = fdhFactory;
        _pathToImageConverter = pathToImageConverter;
        _dialogService = dialogService;
        _modPatcher = modPatcher;
        _settingService = settingService;
        _editorModuleOrderSetting = _settingService.Get<EditorModuleOrderSetting>();

        PluginItems = pluginLoader.LoadPlugins(out var loadFailures);
        if (loadFailures?.AnyFailures == true)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load some plugins", loadFailures.ToString()));
        }

        PatchRomCommand = new RelayCommand(PatchRom);

        RegisterModules(modules);

        GoForwardInModuleStackCommand = new RelayCommand(GoForwardInModuleStack, () => _forwardNavigationStack.Count != 0);
        GoBackInModuleStackCommand = new RelayCommand(GoBackInModuleStack, () => _backNavigationStack.Count != 0);
    }

    private ICachedMsgBlockService? _cachedMsgBlockService;
    public async Task SetMod(ModInfo mod)
    {
        Loading = true;
        try
        {
            Mod = mod;
            await Task.Run(() =>
            {
                if (_bannerService != null)
                {
                    _bannerService.ImageSet -= BannerService_ImageSet;
                }
                _modServiceGetter?.Dispose();
                _modServiceGetter = _modKernelFactory.Create(mod);
                _cachedMsgBlockService = _modServiceGetter.Get<ICachedMsgBlockService>();
                _cachedMsgBlockService.RebuildCache();
                _bannerService = _modServiceGetter.Get<IBannerService>();
                _bannerService.ImageSet += BannerService_ImageSet;
            });
            var module = CurrentModuleId ?? ListItems.FirstOrDefault()?.ModuleId;
            NavigateInternal(module, null, true);
            RaiseAllPropertiesChanged();
        }
        finally
        {
            Loading = false;
        }
        
    }

    private void BannerService_ImageSet()
    {
        RaisePropertyChanged(nameof(Banner));
    }

    public object? Banner
    {
        get
        {
            if (Mod == null)
            {
                return null;
            }
            return _pathToImageConverter.TryConvert(Path.Combine(Mod.FolderPath, Core.Services.Constants.BannerImageFile));
        }
    }

    public bool Loading
    {
        get;
        set => SetProperty(ref field, value);
    }

    public object? CurrentVm
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public string? CurrentModuleId
    {
        get => _currentModule?.UniqueId;
        set
        {
            if (_currentModule?.UniqueId != value)
            {
                NavigateTo(value);
            }
        }
    }

    private EditorModuleListItem? _selectedModuleItem;
    public EditorModuleListItem? SelectedModuleItem
    {
        get => _selectedModuleItem;
        set
        {
            if (SetProperty(ref _selectedModuleItem, value))
            {
                NavigateTo(value?.ModuleId);
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }

    public ModInfo? Mod
    {
        get => _mod;
        private set => SetProperty(ref _mod, value);
    }
    public ObservableCollection<EditorModuleListItem> ListItems { get; } = [];
    private Dictionary<string, EditorModule> UninitialisedModules { get; } = [];
    private Dictionary<string, EditorModule> InitialisedModules { get; } = [];

    #region Naviation

    private readonly Stack<NavigationStackItem> _backNavigationStack = [];
    private readonly Stack<NavigationStackItem> _forwardNavigationStack = [];

    public RelayCommand GoForwardInModuleStackCommand { get; }
    public RelayCommand GoBackInModuleStackCommand { get; }

    private void GoForwardInModuleStack()
    {
        if (_forwardNavigationStack.Count != 0 && CurrentModuleId != null)
        {
            _backNavigationStack.Push(GetCurrentModuleAsStackItem());
            var forwardTo = _forwardNavigationStack.Pop();
            NavigateInternal(forwardTo.ModuleId, forwardTo.SelectedId);
            GoForwardInModuleStackCommand.RaiseCanExecuteChanged();
            GoBackInModuleStackCommand.RaiseCanExecuteChanged();
        }
    }
    private void GoBackInModuleStack()
    {
        if (_backNavigationStack.Count != 0 && CurrentModuleId != null)
        {
            _forwardNavigationStack.Push(GetCurrentModuleAsStackItem());
            var backTo = _backNavigationStack.Pop();
            NavigateInternal(backTo.ModuleId, backTo.SelectedId);
            GoForwardInModuleStackCommand.RaiseCanExecuteChanged();
            GoBackInModuleStackCommand.RaiseCanExecuteChanged();
        }
    }

    private record NavigationStackItem(string? ModuleId, int? SelectedId);

    NavigationStackItem GetCurrentModuleAsStackItem()
    {
        return new NavigationStackItem(_currentModule?.UniqueId, (_currentModule as ISelectableModule)?.SelectedId);
    }

    public void NavigateTo(string? moduleId, int? selectedId = null)
    {
        if (_currentModule?.UniqueId != null)
        {
            _backNavigationStack.Push(GetCurrentModuleAsStackItem());
        }
        _forwardNavigationStack.Clear();
        GoBackInModuleStackCommand.RaiseCanExecuteChanged();
        GoForwardInModuleStackCommand.RaiseCanExecuteChanged();

        NavigateInternal(moduleId, selectedId);
    }

    private void NavigateInternal(string? moduleId, int? selectedId, bool forceUpdate = false)
    {
        if (moduleId == null)
        {
            return;
        }
        if (!TryGetModule(moduleId, out var module))
        {
            return;
        }
        if (selectedId != null && module is ISelectableModule selectableModule)
        {
            selectableModule.Select(selectedId.Value);
        }

        if (_currentModule?.UniqueId == moduleId && !forceUpdate)
        {
            return;
        }

        _currentModule?.OnPageClosing();
        _currentModule = module;
        CurrentVm = _currentModule.ViewModel;
        _currentModule?.OnPageOpening();

        _selectedModuleItem = ListItems.FirstOrDefault(x => x.ModuleId == moduleId);
        RaisePropertyChanged(nameof(SelectedModuleItem));
    }

    #endregion

    private void SaveTextChanges()
    {
        _cachedMsgBlockService = _modServiceGetter?.Get<ICachedMsgBlockService>();
        _cachedMsgBlockService?.SaveChangedBlocks();
    }

    public bool TryGetModule(string moduleId, [NotNullWhen(true)] out EditorModule? module)
    {
        ArgumentNullException.ThrowIfNull(moduleId);
        if (!InitialisedModules.TryGetValue(moduleId, out module))
        {
            if (!UninitialisedModules.TryGetValue(moduleId, out module))
            {
                return false;
            }
            if (_modServiceGetter == null)
            {
                return false;
            }
            module.Initialise(_modServiceGetter);
            InitialisedModules.Add(moduleId, module);
            UninitialisedModules.Remove(moduleId);
        }
        return true;
    }

    private void SelectableModule_RequestNavigate(EditorModule sender, int selectId)
    {
        NavigateTo(sender.UniqueId, selectId);
    }


    #region Module Initialisation

    private void AddModule(EditorModule module)
    {
        if (InitialisedModules.ContainsKey(module.UniqueId) || UninitialisedModules.ContainsKey(module.UniqueId))
        {
            throw new Exception($"Module with ID '{module.UniqueId}' already added to main editor");
        }
        UninitialisedModules.Add(module.UniqueId, module);
        ListItems.Add(new EditorModuleListItem(module.ListName, module.UniqueId));

        if (module is ISelectableModule selectableModule)
        {
            selectableModule.RequestNavigate += SelectableModule_RequestNavigate;
        }
    }

    private void RegisterModules(IEnumerable<EditorModule> modules)
    {
        foreach (var module in modules)
        {
            if (module.UniqueId == null)
            {
                throw new Exception("A Module ID is null");
            }
            AddModule(module);
        }

        LoadModuleOrderFromSetting();
    }


    private void LoadModuleOrderFromSetting()
    {
        var items = ListItems.ToList();
        var order = _editorModuleOrderSetting.Value;
        ListItems.Clear();
        foreach (var item in order)
        {
            var firstItem = items.FirstOrDefault(i => i.ModuleId == item);
            if (firstItem != null)
            {
                ListItems.Add(firstItem);
                items.Remove(firstItem);
            }
        }
        foreach (var item in items)
        {
            ListItems.Add(item);
        }
    }

    #endregion

    public void Deactivate()
    {
        SaveTextChanges();
        foreach (var module in InitialisedModules.Values.ToArray())
        {
            InitialisedModules.Remove(module.UniqueId);
            module.Deactivate();
            UninitialisedModules.Add(module.UniqueId, module);
        }
        CurrentVm = null;

        _editorModuleOrderSetting.Value = ListItems.Select(i => i.ModuleId).ToArray();
        _settingService.Save();
        _modServiceGetter?.Dispose();
        _modServiceGetter = null;
    }

    #region Rom

    public ICommand PatchRomCommand { get; }

    private async Task PatchRom()
    {
        if (Mod == null)
        {
            return;
        }
        var vm = new ModPatchViewModel(_dialogService, _settingService, Mod, _fdhFactory);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        var canPatch = _modPatcher.CanPatch(Mod, vm.File, vm.PatchOpt);
        if (canPatch.IsFailed)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unable to patch", canPatch.ToString()));
            return;
        }

        Exception? error = null;
        await _dialogService.ProgressDialog(progress =>
        {
            progress?.Report(new ProgressInfo("Saving...", IsIndeterminate: true));
            SaveTextChanges();
            foreach (var module in InitialisedModules.Values)
            {
                module.OnPatchingRom();
            }
            try
            {
                _modPatcher.Patch(Mod, vm.File, vm.PatchOpt, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Writing To Rom",
                message: error.ToString(),
                type: MessageBoxType.Error
            ));
        }
    }

    #endregion

    #region Plugins

    public PluginInfo? SelectedPlugin
    {
        get => null;
        set
        {
            // prevent weird double trigger
            if (PluginPopupOpen)
            {
                PluginPopupOpen = false;
                if (value != null)
                {
                    RunPlugin(value);
                }
            }
        }
    }

    public bool PluginPopupOpen
    {
        get;
        set => SetProperty(ref field, value);
    }

    private async void RunPlugin(PluginInfo chosen)
    {
        if (Mod == null)
        {
            return;
        }
        Deactivate();
        try
        {
            using (var services = _modKernelFactory.Create(Mod))
            {
                await chosen.Plugin.Run(new PluginContext(services));
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.ToString(),
                type: MessageBoxType.Error
                ));
        }
        await SetMod(Mod);
    }

    #endregion
}
