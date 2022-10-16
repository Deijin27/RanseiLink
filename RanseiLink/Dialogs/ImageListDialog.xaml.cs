using RanseiLink.Core.Services;
using RanseiLink.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs;
/// <summary>
/// Interaction logic for ImageListDialog.xaml
/// </summary>
public partial class ImageListDialog : Window
{
    public ImageListDialog()
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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
