using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Views;

/// <summary>
/// Interaction logic for SimpleSelectorView.xaml
/// </summary>
public partial class MsgGridView : UserControl
{
    private static double _verticalScrollPos;
    public MsgGridView()
    {
        InitializeComponent();
        msgDataGrid.Loaded += MsgDataGrid_Loaded;
    }

    private void MsgDataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        var sv = DataGridHelper.FindScrollViewer(msgDataGrid);
        sv.ScrollToVerticalOffset(_verticalScrollPos);
        sv.ScrollChanged += MsgDataGrid_ScrollChanged;
    }

    private void MsgDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _verticalScrollPos = e.VerticalOffset;
    }
}
