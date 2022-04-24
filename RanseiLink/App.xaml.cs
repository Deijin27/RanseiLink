using Autofac;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using RanseiLink.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RanseiLink;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static readonly string Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    public MainWindowViewModel GetMainWindowViewModel() => ContainerProvider.Container.Resolve<MainWindowViewModel>();

    public App()
    {
        
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        SetupExceptionHandling();

        // Register services here because theme service requires that application resources are already initialized
        var builder = new ContainerBuilder();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new PluginServiceModule());
        builder.RegisterModule(new WpfServiceModule());
        var container = builder.Build();
        ContainerProvider.Container = container;

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
#if DEBUG
        Debugger.Break();
#endif
        string title = "Unhandled Exception";
        string message = $"Please report this at https://github.com/Deijin27/RanseiLink/issues\n\n{exception}";

        MessageBox.Show(message, title);

        Environment.Exit(0);
    }
}
