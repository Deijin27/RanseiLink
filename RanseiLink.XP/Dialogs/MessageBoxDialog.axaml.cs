using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RanseiLink.XP.Dialogs;
public partial class MessageBoxDialog : Window
{
    public MessageBoxDialog()
    {
        InitializeComponent();

        //DialogWindowBorder.BorderBrush = args.Type switch
        //{
        //    MessageBoxType.Information => new SolidColorBrush(Color.FromRgb(63, 63, 63)),
        //    MessageBoxType.Warning => new SolidColorBrush(Color.FromRgb(242, 194, 0)),
        //    MessageBoxType.Error => new SolidColorBrush(Color.FromRgb(216, 6, 18)),
        //    _ => new SolidColorBrush(Color.FromRgb(63, 63, 63)),
        //};
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var clickedButton = (MessageBoxButton)(((Button)sender).DataContext);
        Result = clickedButton.Result;
        Close();
    }

    public MessageBoxResult Result { get; private set; }

    public static async Task<MessageBoxResult> ShowDialog(MessageBoxSettings options, Window owner)
    {
        var dialog = new MessageBoxDialog()
        {
            DataContext = options
        };
        await dialog.ShowDialog(owner);
        return dialog.Result;
    }
}
