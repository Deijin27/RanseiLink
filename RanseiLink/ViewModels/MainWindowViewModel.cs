using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class MainWindowViewModel : ViewModelBase, ISaveable
{
    private readonly IThemeService _themeService;
    private readonly MainEditorViewModelFactory _mainEditorViewModelFactory;

    public MainWindowViewModel(IServiceContainer container)
    {
        var dialogService = container.Resolve<IDialogService>();
        var pluginLoader = container.Resolve<IPluginLoader>();
        _themeService = container.Resolve<IThemeService>();
        _mainEditorViewModelFactory = container.Resolve<MainEditorViewModelFactory>();

        // Initial load of plugins to create cache and alert user of failures
        pluginLoader.LoadPlugins(out var failures);
        if (failures.AnyFailures)
        {
            dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Plugin Load Failures",
                message: $"Unable to load some of the plugins, details:\n\n{failures}",
                type: MessageBoxType.Warning
                ));
        }

        ModSelectionVm = container.Resolve<ModSelectionViewModel>();
        CurrentVm = ModSelectionVm;

        ModSelectionVm.ModSelected += mi =>
        {
            var mevm = _mainEditorViewModelFactory(mi);
            CurrentVm = mevm;
            BackButtonVisible = true;
        };

        BackButtonCommand = new RelayCommand(() =>
        {
            CurrentVm = ModSelectionVm;
            BackButtonVisible = false;
        });

        ToggleThemeCommand = new RelayCommand(() =>
        {
            var newTheme = _themeService.CurrentTheme switch
            {
                Theme.Dark => Theme.Light,
                Theme.Light => Theme.Dark,
                _ => throw new System.Exception("Invalid theme enum value"),
            };
            _themeService.SetTheme(newTheme);
        });
    }


    private readonly ModSelectionViewModel ModSelectionVm;

    private object currentVm;
    public object CurrentVm
    {
        get => currentVm;
        set
        {
            if (currentVm != value)
            {
                Save();
                currentVm = value;
                RaisePropertyChanged();
            }
        }
    }

    private bool _backButtonVisible;
    public bool BackButtonVisible
    {
        get => _backButtonVisible;
        set => RaiseAndSetIfChanged(ref _backButtonVisible, value);
    }

    public ICommand BackButtonCommand { get; }
    public ICommand ToggleThemeCommand { get; }

    public void OnShutdown()
    {
        Save();
    }

    public void Save()
    {
        if (currentVm is ISaveable saveable)
        {
            saveable.Save();
        }
    }
}
