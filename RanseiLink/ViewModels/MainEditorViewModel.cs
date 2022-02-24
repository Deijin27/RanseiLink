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
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public record EditorModuleListItem(string DisplayName, string ModuleId);

public delegate MainEditorViewModel MainEditorViewModelFactory(ModInfo mod);

public class MainEditorViewModel : ViewModelBase, ISaveable
{
    private readonly IServiceContainer _container;
    private readonly IModServiceContainer _dataService;
    private readonly IDialogService _dialogService;
    private readonly IModManager _modService;
    private readonly IEditorContext _editorContext;
    private readonly ISettingService _settingService;
    private readonly EditorModuleOrderSetting _editorModuleOrderSetting;
    private bool _moduleOrderLoaded = false;
    private bool _pluginPopupOpen = false;
    private string _currentModuleId;
    private ISaveableRefreshable _currentVm;

    public MainEditorViewModel(IServiceContainer container, ModInfo mod)
    {
        _container = container;
        var dataServiceFactory = container.Resolve<DataServiceFactory>();
        var editorContextFactory = container.Resolve<EditorContextFactory>();
        _dialogService = container.Resolve<IDialogService>();
        _modService = container.Resolve<IModManager>();
        _settingService = container.Resolve<ISettingService>();
        _editorModuleOrderSetting = _settingService.Get<EditorModuleOrderSetting>();

        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        Mod = mod;
        _dataService = dataServiceFactory(Mod);
        _editorContext = editorContextFactory(_dataService, this);

        CommitRomCommand = new RelayCommand(CommitRom);

        RegisterModules();
    }

    public ICommand CommitRomCommand { get; }


    public ISaveableRefreshable CurrentVm
    {
        get => _currentVm;
        private set
        {
            Save();
            _currentVm = value;
            _currentVm?.Refresh();
            RaisePropertyChanged();
        }
    }

    public string CurrentModuleId
    {
        get => _currentModuleId;
        set
        {
            if (!CanSave())
            {
                RaisePropertyChanged();
                return;
            }
            if (value != null && RaiseAndSetIfChanged(ref _currentModuleId, value))
            {
                CurrentVm = SelectViewModel(value);
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }
    public ModInfo Mod { get; }
    public ObservableCollection<EditorModuleListItem> ListItems { get; } = new();
    private Dictionary<string, IEditorModule> Modules { get; } = new();
    public Dictionary<string, ISaveableRefreshable> ViewModels { get; } = new();

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

    private void AddModule(IEditorModule module)
    {
        Modules.Add(module.UniqueId, module);
        ViewModels.Add(module.UniqueId, module.NewViewModel(_container, _editorContext));
        ListItems.Add(new(module.ListName, module.UniqueId));
        if (ViewModels.Count == 1)
        {
            _currentModuleId = module.UniqueId;
            CurrentVm = SelectViewModel(_currentModuleId);
        }
    }

    private void RegisterModules()
    {
        var types = System.Reflection.Assembly
                .GetExecutingAssembly()
                .GetTypes();

        IEnumerable<Type> modules = types.Where(i => typeof(IEditorModule).IsAssignableFrom(i) && !i.IsInterface);

        foreach (Type t in modules)
        {
            var module = (IEditorModule)Activator.CreateInstance(t);
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
        _moduleOrderLoaded = true;
    }

    private void ReloadViewModels()
    {
        ViewModels.Clear();
        _editorContext.CachedMsgBlockService.RebuildCache();
        foreach (var (key, module) in Modules)
        {
            ViewModels[key] = module.NewViewModel(_container, _editorContext);
        }
    }

    private ISaveableRefreshable SelectViewModel(string id)
    {
        return ViewModels[id];
    }

    public bool CanSave()
    {
        return CurrentVm?.CanSave() != false;
    }

    public void Save()
    {
        if (_moduleOrderLoaded) // make sure default order isn't saved before saved order is loaded
        {
            _editorModuleOrderSetting.Value = ListItems.Select(i => i.ModuleId).ToArray();
            _settingService.Save();
        }
        _editorContext.CachedMsgBlockService.SaveChangedBlocks();
        CurrentVm?.Save();
    }

    private void CommitRom()
    {
        if (!CanSave())
        {
            return;
        }

        if (!_dialogService.CommitToRom(Mod, out string romPath, out var patchOpt))
        {
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Saving...", IsIndeterminate: true));
            Save();
            try
            {
                _modService.Commit(Mod, romPath, patchOpt, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Error Writing To Rom",
                message: error.Message,
                type: MessageBoxType.Error
            ));
        }
    }

    private void RunPlugin(PluginInfo chosen)
    {

        // first save
        if (!CanSave())
        {
            return;
        }
        Save();

        // then run plugin
        try
        {
            chosen.Plugin.Run(new PluginContext(_container, Mod));
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.Message,
                type: MessageBoxType.Error
                ));
        }
        ReloadViewModels();
        _currentVm = null;
        CurrentVm = SelectViewModel(_currentModuleId);
    }
}
