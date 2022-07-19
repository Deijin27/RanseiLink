using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RanseiLink.Views;

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
        var sv = FindScrollViewer(msgDataGrid);
        sv.ScrollToVerticalOffset(_verticalScrollPos);
        sv.ScrollChanged += MsgDataGrid_ScrollChanged;
    }

    private void MsgDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _verticalScrollPos = e.VerticalOffset;
    }

    private static ScrollViewer FindScrollViewer(DependencyObject d)
    {
        if (d is ScrollViewer sv)
        {
            return sv;
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        {
            sv = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
            if (sv != null)
            {
                return sv;
            }
        }
        return null;
    }
}
