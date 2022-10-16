using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using RanseiLink.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public record EditorModuleListItem(string DisplayName, string ModuleId);

public interface IMainEditorViewModel
{
    void SetMod(ModInfo mod);
    void Deactivate();
    string CurrentModuleId { get; set; }
    bool TryGetModule(string moduleId, out EditorModule module);
}
public class MainEditorViewModel : ViewModelBase, IMainEditorViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IModPatchingService _modPatcher;
    private readonly ISettingService _settingService;
    private readonly IModServiceGetterFactory _modKernelFactory;
    private readonly EditorModuleOrderSetting _editorModuleOrderSetting;

    private bool _pluginPopupOpen = false;
    private object _currentVm;
    private ModInfo _mod;
    private IServiceGetter _modServiceGetter;
    private EditorModule _currentModule;

    public MainEditorViewModel(
        IDialogService dialogService,
        IModPatchingService modPatcher,
        ISettingService settingService,
        IPluginLoader pluginLoader,
        IModServiceGetterFactory modKernelFactory,
        IEnumerable<EditorModule> modules)
    {
        _modKernelFactory = modKernelFactory;
        _dialogService = dialogService;
        _modPatcher = modPatcher;
        _settingService = settingService;
        _editorModuleOrderSetting = _settingService.Get<EditorModuleOrderSetting>();

        PluginItems = pluginLoader.LoadPlugins(out var loadFailures);
        if (loadFailures?.AnyFailures == true)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load some plugins", loadFailures?.ToString()));
        }

        CommitRomCommand = new RelayCommand(CommitRom);

        RegisterModules(modules);

        GoForwardInModuleStackCommand = new RelayCommand(GoForwardInModuleStack, () => _forwardModuleStack.Any());
        GoBackInModuleStackCommand = new RelayCommand(GoBackInModuleStack, () => _backModuleStack.Any());
    }

    private ICachedMsgBlockService _cachedMsgBlockService;
    public void SetMod(ModInfo mod)
    {
        Mod = mod;
        _modServiceGetter?.Dispose();
        _modServiceGetter = _modKernelFactory.Create(mod);
        _cachedMsgBlockService = _modServiceGetter.Get<ICachedMsgBlockService>();
        _cachedMsgBlockService.RebuildCache();
        SetCurrentModule(CurrentModuleId ?? ListItems[0].ModuleId, true);
        RaiseAllPropertiesChanged();
    }

    public object CurrentVm
    {
        get => _currentVm;
        private set => RaiseAndSetIfChanged(ref _currentVm, value);
    }

    public string CurrentModuleId
    {
        get => _currentModule?.UniqueId;
        set
        {
            if (_currentModule?.UniqueId != value)
            {
                SetCurrentModule(value);
            }
        }
    }

    private EditorModuleListItem _selectedModuleItem;
    public EditorModuleListItem SelectedModuleItem
    {
        get => _selectedModuleItem;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedModuleItem, value))
            {
                SetCurrentModule(value?.ModuleId);
                _forwardModuleStack.Clear();
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }

    public ModInfo Mod
    {
        get => _mod;
        private set => RaiseAndSetIfChanged(ref _mod, value);
    }
    public ObservableCollection<EditorModuleListItem> ListItems { get; } = new();
    private Dictionary<string, EditorModule> UninitialisedModules { get; } = new();
    private Dictionary<string, EditorModule> InitialisedModules { get; } = new();

    private readonly Stack<string> _backModuleStack = new();
    private readonly Stack<string> _forwardModuleStack = new();

    public ICommand GoForwardInModuleStackCommand { get; }
    public ICommand GoBackInModuleStackCommand { get; }

    private void GoForwardInModuleStack()
    {
        if (_forwardModuleStack.Any())
        {
            SetCurrentModule(_forwardModuleStack.Pop());
        }
    }
    private void GoBackInModuleStack()
    {
        if (_backModuleStack.Any())
        {
            _forwardModuleStack.Push(CurrentModuleId);
            SetCurrentModule(_backModuleStack.Pop(), blockStackPush:true);
        }
    }

    private void SaveTextChanges()
    {
        _cachedMsgBlockService = _modServiceGetter.Get<ICachedMsgBlockService>();
        _cachedMsgBlockService.SaveChangedBlocks();
    }

    public bool TryGetModule(string moduleId, out EditorModule module)
    {
        if (moduleId == null)
        {
            throw new ArgumentNullException(nameof(moduleId));
        }
        if (!InitialisedModules.TryGetValue(moduleId, out module))
        {
            if (!UninitialisedModules.TryGetValue(moduleId, out module))
            {
                return false;
            }
            module.Initialise(_modServiceGetter);
            InitialisedModules.Add(moduleId, module);
            UninitialisedModules.Remove(moduleId);
        }
        return true;
    }

    private void SetCurrentModule(string moduleId, bool forceUpdate = false, bool blockStackPush = false)
    {
        if (moduleId == null)
        {
            // I have no idea why, but randomly a null value is set sometimes
            return;
        }
        if (_currentModule?.UniqueId == moduleId && !forceUpdate)
        {
            return;
        }
        if (!TryGetModule(moduleId, out var module))
        {
            return;
        }
        if (_currentModule != null && _currentModule.UniqueId != moduleId && !blockStackPush)
        {
            _backModuleStack.Push(_currentModule.UniqueId);
        }
        _currentModule?.OnPageClosing();
        _currentModule = module;
        CurrentVm = _currentModule.ViewModel;
        _currentModule?.OnPageOpening();

        _selectedModuleItem = ListItems.FirstOrDefault(x => x.ModuleId == moduleId);
        RaisePropertyChanged(nameof(SelectedModuleItem));
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
    }

    #region Rom

    public ICommand CommitRomCommand { get; }

    private void CommitRom()
    {
        var vm = new ModCommitViewModel(_dialogService, _settingService, Mod);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        var canPatch = _modPatcher.CanPatch(Mod, vm.File, vm.PatchOpt);
        if (!canPatch.CanPatch)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unable to patch", canPatch.ReasonCannotPatch));
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress?.Report(new ProgressInfo("Saving...", isIndeterminate: true));
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
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Writing To Rom",
                message: error.ToString(),
                type: MessageBoxType.Error
            ));
        }
    }

    #endregion

    #region Plugins

    public PluginInfo SelectedPlugin
    {
        get => null;
        set
        {
            // prevent weird double trigger
            if (PluginPopupOpen)
            {
                PluginPopupOpen = false;
                RunPlugin(value);
            }
        }
    }

    public bool PluginPopupOpen
    {
        get => _pluginPopupOpen;
        set => RaiseAndSetIfChanged(ref _pluginPopupOpen, value);
    }

    private void RunPlugin(PluginInfo chosen)
    {
        try
        {
            chosen.Plugin.Run(new PluginContext(_modServiceGetter));
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.ToString(),
                type: MessageBoxType.Error
                ));
        }
        foreach (var module in InitialisedModules.Values)
        {
            module.OnPluginComplete();
        }
    }

    #endregion
}
