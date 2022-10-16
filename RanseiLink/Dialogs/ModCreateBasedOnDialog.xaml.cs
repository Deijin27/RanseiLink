using RanseiLink.Core.Services;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs;

/// <summary>
/// Interaction logic for ModCreationDialog.xaml
/// </summary>
public partial class ModCreateBasedOnDialog : Window
{
    public ModCreateBasedOnDialog()
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
