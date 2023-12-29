using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore;
using RanseiLink.PluginModule;
using RanseiLink.View3D;
using System.Diagnostics;
using System.Reflection;

namespace RanseiLink.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static readonly string Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    public MainWindowViewModel GetMainWindowViewModel() => _mainWindowViewModel;
    private MainWindowViewModel _mainWindowViewModel;
    private static IDialogService _dialogService;
    public ISettingService SettingService { get; private set; }
    

    public App()
    {
        
    }

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        SetupExceptionHandling();

        // Register services here because theme service requires that application resources are already initialized
        var builder = new Container();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new GuiCoreServiceModule());
        builder.RegisterModule(new PluginServiceModule());
        builder.RegisterModule(new WpfServiceModule());

        var modServiceGetter = new ModServiceGetterFactory(builder);
        modServiceGetter.AddModule(new CoreModServiceModule());
        modServiceGetter.AddModule(new GuiCoreModServiceModule());
        modServiceGetter.AddModule(new View3DModServiceModule());
        modServiceGetter.AddModule(new WpfModServiceModule());
        _dialogService = builder.Resolve<IDialogService>() ?? throw new Exception("Missing dialog service");
        builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

        _mainWindowViewModel = builder.Resolve<MainWindowViewModel>() ?? throw new Exception("Missing main window");
        SettingService = builder.Resolve<ISettingService>() ?? throw new Exception("Missing setting service");
        

        base.OnStartup(e);
    }

    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
    }

    private static void LogUnhandledException(Exception exception, string source)
    {
        if (Debugger.IsAttached)
        {
            return;
        }
        string title = "Unhandled Exception";
        string exMsg = $"{source}:\n{exception}";
        string message = $"Please report this at https://github.com/Deijin27/RanseiLink/issues\n\n{exMsg}";
        
        if (_dialogService != null)
        {
            try
            {
                var result = _dialogService.ShowMessageBox(new MessageBoxSettings(
                    Title: title,
                    Message: message,
                    Buttons: [
                        new("Open Bug Report Webpage", MessageBoxResult.Yes),
                        new("Dismiss", MessageBoxResult.No) ],
                    Type: MessageBoxType.Error
                    ));
                if (result == MessageBoxResult.Yes)
                {
                    IssueReporter.ReportCrash(App.Version, exMsg);
                }
            }
            catch
            {
                System.Windows.MessageBox.Show(message, title);
            }
        }
        else
        {
            System.Windows.MessageBox.Show(message, title);
        }

        Environment.Exit(0);
    }
}
