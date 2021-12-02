using RanseiLink.ViewModels;
using System.Windows;

namespace RanseiLink;

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

        DataContext = new MainWindowViewModel(((App)Application.Current).ServiceContainer);
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.OnShutdown();
        App.Current.Shutdown();
    }

    private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeWindowButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

}
