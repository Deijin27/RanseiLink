using RanseiLink.Core.Services;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.Dialogs;

/// <summary>
/// Interaction logic for MessageBoxDialog.xaml
/// </summary>
public partial class MessageBoxDialog : Window
{
    public MessageBoxDialog(MessageBoxSettings args)
    {
        InitializeComponent();
        DataContext = args;

        DialogWindowBorder.BorderBrush = args.Type switch
        {
            MessageBoxType.Information => new SolidColorBrush(Color.FromRgb(63, 63, 63)),
            MessageBoxType.Warning => new SolidColorBrush(Color.FromRgb(242, 194, 0)),
            MessageBoxType.Error => new SolidColorBrush(Color.FromRgb(216, 6, 18)),
            _ => new SolidColorBrush(Color.FromRgb(63, 63, 63)),
        };
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
        var clickedButton = (Core.Services.MessageBoxButton)(((System.Windows.Controls.Button)sender).DataContext);
        Result = clickedButton.Result;
        Close();
    }

    public Core.Services.MessageBoxResult Result { get; private set; }
}
