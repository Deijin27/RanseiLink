using RanseiLink.Core.Services;
using RanseiLink.GuiCore.Constants;
using RanseiLink.PluginModule.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;
    private readonly IModSelectionViewModel _modSelectionVm;
    private readonly IMainEditorViewModel _mainEditorViewModel;
    private readonly IFallbackSpriteManager _fallbackManager;
    private object _currentVm;
    private bool _backButtonVisible;

    public MainWindowViewModel(
        IAsyncDialogService dialogService,
        IPluginLoader pluginLoader, 
        IThemeService themeService,
        IModSelectionViewModel modSelectionViewModel,
        IMainEditorViewModel mainEditorViewModel,
        IFallbackSpriteManager fallbackManager)
    {
        _fallbackManager = fallbackManager;
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

        _themeService = themeService;
        _modSelectionVm = modSelectionViewModel;
        _mainEditorViewModel = mainEditorViewModel;
        _currentVm = _modSelectionVm;

        _modSelectionVm.ModSelected += OnModSelected;

        BackButtonCommand = new RelayCommand(OnBackButtonPressed);
        ToggleThemeCommand = new RelayCommand(() =>
        {
            themeService.ToggleTheme();
            RaisePropertyChanged(nameof(ThemeIcon));
        });
    }

    public async void OnWindowShown()
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
        set => SetProperty(ref _backButtonVisible, value);
    }

    public ICommand BackButtonCommand { get; }
    public ICommand ToggleThemeCommand { get; }

    public IconId ThemeIcon => _themeService.ThemeIcon();

    public void OnShutdown()
    {
        if (_currentVm is IMainEditorViewModel mainEditor)
        {
            mainEditor.Deactivate();
        }
    }

    public bool MainEditorEnabled { get; set; } = true;

    private async void OnModSelected(ModInfo mod)
    {
        if (!MainEditorEnabled)
        {
            return;
        }
        var task = _mainEditorViewModel.SetMod(mod);
        CurrentVm = _mainEditorViewModel;
        await task;
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

        foreach (var item in _modSelectionVm.AllItems)
        {
            item.UpdateBanner();
        }
    }
}
