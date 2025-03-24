using Avalonia;
using RanseiLink.XP.Dialogs;
using System.Diagnostics;

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
            Debugger.Break();
            //return;
        }
        string title = "Unhandled Exception";
        string exMsg = $"{source}:\n{exception}";
        string message = $"Please report this at https://github.com/Deijin27/RanseiLink/issues\n\n{exMsg}";

        if (App.MainWindow != null)
        {
            var options = new MessageBoxSettings(
                Title: title,
                Message: message,
                Buttons: new[] {
                    new MessageBoxButton("Open Bug Report Webpage", MessageBoxResult.Yes),
                    new MessageBoxButton("Dismiss", MessageBoxResult.No) },
                Type: MessageBoxType.Error
                );

            // Can't await because the app just closes, so use synchronous dialog
            var result = MessageBoxDialog.ShowDialog(options, App.MainWindow).WaitOnDispatcherFrame();

            if (result == MessageBoxResult.Yes)
            {
                IssueReporter.ReportCrash(App.Version, exMsg);
            }
        }
        else
        {
            IssueReporter.ReportCrash(App.Version, exMsg);
        }

        Environment.Exit(0);
    }
}
