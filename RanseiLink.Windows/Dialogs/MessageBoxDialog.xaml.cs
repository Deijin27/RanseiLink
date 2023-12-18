using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.Windows.Dialogs;

/// <summary>
/// Interaction logic for MessageBoxDialog.xaml
/// </summary>
public partial class MessageBoxDialog : System.Windows.Window
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

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var clickedButton = (MessageBoxButton)(((System.Windows.Controls.Button)sender).DataContext);
        Result = clickedButton.Result;
        Close();
    }

    public MessageBoxResult Result { get; private set; }
}
