using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RanseiLink.XP.Dialogs;
public partial class ImageListDialog : Window
{
    public ImageListDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
