using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RanseiLink.XP.Views;
public partial class PluginDialog : Window
{
    public PluginDialog()
    {
        InitializeComponent();
    }

    public bool DialogResult { get; set; }

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
