using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
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

        _modSelectionVm = modSelectionViewModel;
        _mainEditorViewModel = mainEditorViewModel;
        _currentVm = _modSelectionVm;

        _modSelectionVm.ModSelected += OnModSelected;

        BackButtonCommand = new RelayCommand(OnBackButtonPressed);
        ToggleThemeCommand = new RelayCommand(themeService.ToggleTheme);
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
                Notify();
            }
        }
    }

    
    public bool BackButtonVisible
    {
        get => _backButtonVisible;
        set => Set(ref _backButtonVisible, value);
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

    public bool MainEditorEnabled { get; set; } = true;

    private void OnModSelected(ModInfo mod)
    {
        if (!MainEditorEnabled)
        {
            return;
        }
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

        foreach (var item in _modSelectionVm.AllItems)
        {
            item.UpdateBanner();
        }
    }
}
