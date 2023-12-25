using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RanseiLink.XP.Dialogs;
public partial class ModDeleteDialog : Window
{
    public ModDeleteDialog()
    {
        InitializeComponent();
    }

    private IModalDialogViewModel ViewModel => DataContext as IModalDialogViewModel;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.OnClosing(true);
        Close(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.OnClosing(false);
        Close(false);

    }
}
