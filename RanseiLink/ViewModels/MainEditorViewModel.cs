using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public record ListItem(string ItemName, string ItemValue);

public delegate MainEditorViewModel MainEditorViewModelFactory(ModInfo mod);
public delegate void EditorModuleRegistrationFunction(MainEditorViewModel editor);

public class MainEditorViewModel : ViewModelBase, ISaveable
{
    private readonly IServiceContainer _container;
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly IModService _modService;
    private readonly IEditorContext _editorContext;
    private readonly ISettingsService _settingsService;

    public ICommand CommitRomCommand { get; }

    private ISaveableRefreshable _currentVm;
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

    private string _currentPage;
    public string CurrentPage
    {
        get => _currentPage;
        set
        {
            if (!CanSave())
            {
                RaisePropertyChanged();
                return;
            }
            if (value != null && RaiseAndSetIfChanged(ref _currentPage, value))
            {
                CurrentVm = SelectViewModel(value);
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }
    public ModInfo Mod { get; }
    public ObservableCollection<ListItem> ListItems { get; } = new();

    public MainEditorViewModel(IServiceContainer container, ModInfo mod)
    {
        _container = container;
        var dataServiceFactory = container.Resolve<DataServiceFactory>();
        var editorContextFactory = container.Resolve<EditorContextFactory>();
        _dialogService = container.Resolve<IDialogService>();
        _modService = container.Resolve<IModService>();
        _settingsService = container.Resolve<ISettingsService>();

        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        Mod = mod;
        _dataService = dataServiceFactory(Mod);
        _editorContext = editorContextFactory(_dataService, this);

        CommitRomCommand = new RelayCommand(CommitRom);
    }

    /// <summary>
    /// Add <see cref="IEditorModule"/>
    /// </summary>
    /// <param name="t"><see cref="IEditorModule"/> assignable type with blank constructor</param>
    public void AddModule(Type t)
    {
        if (!typeof(IEditorModule).IsAssignableFrom(t))
        {
            return;
        }
        var module = (IEditorModule)Activator.CreateInstance(t);
        Modules.Add(module.UniqueId, module);
        ViewModels.Add(module.UniqueId, module.NewViewModel(_container, _editorContext));
        ListItems.Add(new(module.ListName, module.UniqueId));
        if (ViewModels.Count == 1)
        {
            _currentPage = module.UniqueId;
            CurrentVm = SelectViewModel(_currentPage);
        }
    }

    private bool _moduleOrderLoaded = false;
    public void LoadModuleOrderFromSetting()
    {
        var items = ListItems.ToList();
        ICollection<string> order = _settingsService.EditorModuleOrder;
        ListItems.Clear();
        foreach (var item in order)
        {
            var firstItem = items.FirstOrDefault(i => i.ItemValue == item);
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

    public Dictionary<string, IEditorModule> Modules { get; } = new();
    public Dictionary<string, ISaveableRefreshable> ViewModels { get; } = new();

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
            _settingsService.EditorModuleOrder = ListItems.Select(i => i.ItemValue).ToArray();
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

        if (!_dialogService.CommitToRom(Mod, out string romPath))
        {
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Saving..."));
            Save();
            progress.Report(new ProgressInfo("Patching Rom...", 20));
            try
            {
                _modService.Commit(Mod, romPath);
            }
            catch (Exception e)
            {
                error = e;
            }
            progress.Report(new ProgressInfo("Patching Complete...", 100));
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
        CurrentVm = SelectViewModel(_currentPage);
    }

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

    private bool _pulginPopupOpen = false;
    public bool PluginPopupOpen
    {
        get => _pulginPopupOpen;
        set => RaiseAndSetIfChanged(ref _pulginPopupOpen, value);
    }
}
