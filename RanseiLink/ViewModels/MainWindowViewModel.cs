using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class MainWindowViewModel : ViewModelBase, ISaveable
{
    private readonly IThemeService _themeService;
    private readonly MainEditorViewModelFactory _mainEditorViewModelFactory;
    private readonly ModSelectionViewModel _modSelectionVm;
    private object _currentVm;
    private bool _backButtonVisible;

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

        _modSelectionVm = container.Resolve<ModSelectionViewModel>();
        CurrentVm = _modSelectionVm;

        _modSelectionVm.ModSelected += OnModSelected;

        BackButtonCommand = new RelayCommand(OnBackButtonPressed);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);
    }

    public object CurrentVm
    {
        get => _currentVm;
        set
        {
            if (_currentVm != value)
            {
                Save();
                _currentVm = value;
                RaisePropertyChanged();
            }
        }
    }

    
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
        if (_currentVm is ISaveable saveable)
        {
            saveable.Save();
        }
    }

    private void ToggleTheme()
    {
        var newTheme = _themeService.CurrentTheme switch
        {
            Theme.Dark => Theme.Light,
            Theme.Light => Theme.Dark,
            _ => throw new System.Exception("Invalid theme enum value"),
        };
        _themeService.SetTheme(newTheme);
    }

    private void OnModSelected(ModInfo mod)
    {
        var mevm = _mainEditorViewModelFactory(mod);
        CurrentVm = mevm;
        BackButtonVisible = true;
    }

    private void OnBackButtonPressed()
    {
        CurrentVm = _modSelectionVm;
        BackButtonVisible = false;
    }
}
