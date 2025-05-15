using System.Windows.Controls;

namespace RanseiLink.Windows.Views;

/// <summary>
/// Interaction logic for MainEditorView.xaml
/// </summary>
public partial class MainEditorView : UserControl
{
    public MainEditorView()
    {
        InitializeComponent();
    }

    private void BannerImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        (this.DataContext as IMainEditorViewModel)?.NavigateTo(BannerEditorModule.Id);
    }
}
