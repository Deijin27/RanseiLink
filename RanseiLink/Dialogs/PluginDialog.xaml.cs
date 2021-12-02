using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs;

/// <summary>
/// Interaction logic for PluginDialog.xaml
/// </summary>
public partial class PluginDialog : Window
{
    public PluginDialog()
    {
        InitializeComponent();
    }

    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();

    }
}
