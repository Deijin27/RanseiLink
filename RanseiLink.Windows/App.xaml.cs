using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.PluginModule;
using RanseiLink.View3D;
using RanseiLink.Windows.Services;
using RanseiLink.Windows.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RanseiLink.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static readonly string Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    public MainWindowViewModel GetMainWindowViewModel() => _mainWindowViewModel;
    private MainWindowViewModel _mainWindowViewModel;
    private static IDialogService _dialogService;
    public ISettingService SettingService { get; private set; }
    

    public App()
    {
        
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        SetupExceptionHandling();

        // Register services here because theme service requires that application resources are already initialized
        var builder = new Container();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new PluginServiceModule());
        builder.RegisterModule(new WpfServiceModule());

        var modServiceGetter = new ModServiceGetterFactory(builder);
        modServiceGetter.AddModule(new CoreModServiceModule());
        modServiceGetter.AddModule(new View3DModServiceModule());
        modServiceGetter.AddModule(new WpfModServiceModule());
        _dialogService = builder.Resolve<IDialogService>();
        builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

        _mainWindowViewModel = builder.Resolve<MainWindowViewModel>();
        SettingService = builder.Resolve<ISettingService>();
        

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
                    title: title,
                    message: message,
                    buttons: new[] {
                        new Core.Services.MessageBoxButton("Open Bug Report Webpage", Core.Services.MessageBoxResult.Yes),
                        new Core.Services.MessageBoxButton("Dismiss", Core.Services.MessageBoxResult.No) },
                    type: MessageBoxType.Error
                    ));
                if (result == Core.Services.MessageBoxResult.Yes)
                {
                    IssueReporter.ReportCrash(exMsg);
                }
            }
            catch
            {
                MessageBox.Show(message, title);
            }
        }
        else
        {
            MessageBox.Show(message, title);
        }

        Environment.Exit(0);
    }
}
