using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Views;

/// <summary>
/// Interaction logic for WorkspaceView.xaml
/// </summary>
public partial class WorkspaceView : UserControl
{
    public WorkspaceView()
    {
        InitializeComponent();
        DataContextChanged += WorkspaceView_DataContextChanged;
    }

    private void WorkspaceView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is WorkspaceViewModel vm)
        {
            if (vm.ScrollBig)
            {
                BigViewModelHost.ContentTemplate = (DataTemplate)FindResource("BigViewScrollHostTemplate");
            }
            else
            {
                BigViewModelHost.ContentTemplate = null;
            }
        }
    }
}
