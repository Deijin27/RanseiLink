using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Windows.Services;

namespace RanseiLink.Windows.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;
    private readonly IModSelectionViewModel _modSelectionVm;
    private readonly IMainEditorViewModel _mainEditorViewModel;
    private readonly IFallbackSpriteManager _fallbackManager;
    private object _currentVm;
    private bool _backButtonVisible;

    public MainWindowViewModel(
        IDialogService dialogService,
        IPluginLoader pluginLoader, 
        IThemeService themeService,
        IModSelectionViewModel modSelectionViewModel,
        IMainEditorViewModel mainEditorViewModel,
        IFallbackSpriteManager fallbackManager)
    {
        _fallbackManager = fallbackManager;
        _themeService = themeService;
        // Initial load of plugins to create cache and alert user of failures
        pluginLoader.LoadPlugins(out var failures);
        if (failures?.AnyFailures == true)
        {
            dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Plugin Load Failures",
                message: $"Unable to load some of the plugins, details:\n\n{failures}",
                type: MessageBoxType.Warning
                ));
        }

        _modSelectionVm = modSelectionViewModel;
        _mainEditorViewModel = mainEditorViewModel;
        CurrentVm = _modSelectionVm;

        _modSelectionVm.ModSelected += OnModSelected;

        BackButtonCommand = new RelayCommand(OnBackButtonPressed);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);
    }

    internal async void OnWindowShown()
    {
        await _fallbackManager.CheckDefaultsPopulated();
    }

    public object CurrentVm
    {
        get => _currentVm;
        set
        {
            if (_currentVm != value)
            {
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
        if (_currentVm is IMainEditorViewModel mainEditor)
        {
            mainEditor.Deactivate();
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
        _mainEditorViewModel.SetMod(mod);
        CurrentVm = _mainEditorViewModel;
        BackButtonVisible = true;
    }

    private void OnBackButtonPressed()
    {
        if (_currentVm is IMainEditorViewModel mainEditor)
        {
            mainEditor.Deactivate();
        }
        CurrentVm = _modSelectionVm;
        BackButtonVisible = false;

        foreach (var item in _modSelectionVm.ModItems)
        {
            item.UpdateBanner();
        }
    }
}
