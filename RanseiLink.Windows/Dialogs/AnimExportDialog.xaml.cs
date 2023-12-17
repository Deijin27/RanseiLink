using RanseiLink.Core.Services;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Windows.Dialogs;

public partial class AnimExportDialog : Window
{
    public AnimExportDialog()
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

    private IModalDialogViewModel ViewModel => DataContext as IModalDialogViewModel;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.OnClosing(true);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.OnClosing(false);
        Close();

    }
}
