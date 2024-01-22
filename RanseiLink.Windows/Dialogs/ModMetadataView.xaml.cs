using System.Windows.Controls;

namespace RanseiLink.Windows.Dialogs;
/// <summary>
/// Interaction logic for ModMetadataView.xaml
/// </summary>
public partial class ModMetadataView : UserControl
{
    public ModMetadataView()
    {
        InitializeComponent();
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var vm = DataContext as ModMetadataViewModelBase;
            vm?.AddTagCommand.Execute(null);
        }
    }
}
