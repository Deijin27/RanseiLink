using Avalonia;
using RanseiLink.Core.Services;
using RanseiLink.XP.Dialogs;
using RanseiLink.XP.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RanseiLink.XP;
internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        SetupExceptionHandling();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }

    private static void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

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
            //return;
        }
        string title = "Unhandled Exception";
        string exMsg = $"{source}:\n{exception}";
        string message = $"Please report this at https://github.com/Deijin27/RanseiLink/issues\n\n{exMsg}";

        if (App.MainWindow != null)
        {
            var options = new MessageBoxSettings(
                title: title,
                message: message,
                buttons: new[] {
                    new Core.Services.MessageBoxButton("Open Bug Report Webpage", Core.Services.MessageBoxResult.Yes),
                    new Core.Services.MessageBoxButton("Dismiss", Core.Services.MessageBoxResult.No) },
                type: MessageBoxType.Error
                );

            // Can't await because the app just closes, so use synchronous dialog
            var result = MessageBoxDialog.ShowDialog(options, App.MainWindow).WaitOnDispatcherFrame();

            if (result == Core.Services.MessageBoxResult.Yes)
            {
                IssueReporter.ReportCrash(exMsg);
            }
        }
        else
        {
            IssueReporter.ReportCrash(exMsg);
        }

        Environment.Exit(0);
    }
}
