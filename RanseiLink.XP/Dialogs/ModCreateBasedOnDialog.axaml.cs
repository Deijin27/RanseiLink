using Avalonia.Controls;
using Avalonia.Interactivity;
using RanseiLink.Core.Services;

namespace RanseiLink.XP.Dialogs;
public partial class ModCreateBasedOnDialog : Window
{
    public ModCreateBasedOnDialog()
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
