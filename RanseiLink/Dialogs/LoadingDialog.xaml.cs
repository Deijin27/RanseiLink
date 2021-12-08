using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs;

/// <summary>
/// Interaction logic for ModCreationDialog.xaml
/// </summary>
public partial class LoadingDialog : Window
{
    public LoadingDialog()
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

}
