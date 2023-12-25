using RanseiLink.Core.Settings;
using RanseiLink.Windows.ViewModels;
using System.Windows;

namespace RanseiLink.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Ensure maximized window doesn't cover taskbar
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        var app = (App)Application.Current;
        RestoreDimensions(app.SettingService);
        
        DataContext = app.GetMainWindowViewModel();
    }

    private void RestoreDimensions(ISettingService settingService)
    {
        var dims = settingService.Get<WindowDimensionsSetting>().Value;
        if (dims.X > 0)
        {
            this.Left = dims.X;
        }
        if (dims.Y > 0)
        {
            this.Top = dims.Y;
        }
        if (dims.Width > 0)
        {
            this.Width = dims.Width;
        }
        if (dims.Height > 0)
        {
            this.Height = dims.Height;
        }
        if (dims.State == GuiCore.Settings.WindowState.Normal)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }
        else if (dims.State == GuiCore.Settings.WindowState.Maximized)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
    }

    private void SaveDimensions(ISettingService settingService)
    {
        var dimSetting = settingService.Get<WindowDimensionsSetting>();
        dimSetting.Value = new WindowDimensions(Left, Top, Width, Height, 
            WindowState == System.Windows.WindowState.Maximized ? GuiCore.Settings.WindowState.Maximized : GuiCore.Settings.WindowState.Normal);
        settingService.Save();
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        SaveDimensions(app.SettingService);
        ViewModel.OnShutdown();
        App.Current.Shutdown();
    }

    private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = System.Windows.WindowState.Minimized;
    }

    private void MaximizeWindowButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == System.Windows.WindowState.Maximized
            ? System.Windows.WindowState.Normal
            : System.Windows.WindowState.Maximized;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (!Shown)
        {
            ViewModel.OnWindowShown();
        }
        Shown = true;
    }

    public bool Shown { get; private set; }

}
