using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Views;

/// <summary>
/// Interaction logic for SimpleSelectorView.xaml
/// </summary>
public partial class ScenarioWarriorGridView : UserControl
{
    private static double _verticalScrollPos;

    public ScenarioWarriorGridView()
    {
        InitializeComponent();
        warriorsDataGrid.Loaded += DataGrid_Loaded;
    }

    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        var sv = DataGridHelper.FindScrollViewer(warriorsDataGrid);
        sv.ScrollToVerticalOffset(_verticalScrollPos);
        sv.ScrollChanged += DataGrid_ScrollChanged;
    }

    private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _verticalScrollPos = e.VerticalOffset;
    }
}
