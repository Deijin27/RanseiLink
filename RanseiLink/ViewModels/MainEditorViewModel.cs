using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (RaiseAndSetIfChanged(ref _currentPage, value))
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

        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        Mod = mod;
        _dataService = dataServiceFactory(Mod);
        _editorContext = editorContextFactory(_dataService, this);

        CommitRomCommand = new RelayCommand(CommitRom);
    }

    public void AddModule<TModule>() where TModule : IEditorModule, new()
    {
        var module = new TModule();
        Modules.Add(module.UniqueId, module);
        ViewModels.Add(module.UniqueId, module.NewViewModel(_container, _editorContext));
        ListItems.Add(new(module.ListName, module.UniqueId));
        if (ViewModels.Count == 1)
        {
            _currentPage = module.UniqueId;
            CurrentVm = SelectViewModel(_currentPage);
        }
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
        _dialogService.ProgressDialog(async (text, number) =>
        {
            text.Report("Saving...");
            Save();
            number.Report(20);
            text.Report("Patching rom...");
            try
            {
                await Task.Run(() => _modService.Commit(Mod, romPath));
            }
            catch (Exception e)
            {
                error = e;
            }
            number.Report(100);
            text.Report("Patching Complete!");
            await Task.Delay(500);
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
