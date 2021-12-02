using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class MainWindowViewModel : ViewModelBase, ISaveable
{
    public MainWindowViewModel(IServiceContainer container)
    {
        var dialogService = container.Resolve<IDialogService>();
        var pluginLoader = container.Resolve<IPluginLoader>();

        // Initial load of plugins to create cache and alert user of failures
        pluginLoader.LoadPlugins(out var failures);
        if (failures.AnyFailures)
        {
            dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Plugin Load Failures",
                message: $"Unable to load some of the plugins, details:\n\n{failures}",
                icon: MessageBoxIcon.Warning
                ));
        }

        ModSelectionVm = new ModSelectionViewModel(container);
        CurrentVm = ModSelectionVm;

        ModSelectionVm.ModSelected += mi =>
        {
            var mevm = new MainEditorViewModel(container, mi);
            CurrentVm = mevm;
            BackButtonVisible = true;
        };

        BackButtonCommand = new RelayCommand(() =>
        {
            CurrentVm = ModSelectionVm;
            BackButtonVisible = false;
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
