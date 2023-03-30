using RanseiLink.Windows.ViewModels;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace RanseiLink.Windows.Views;

public partial class MapView : UserControl
{
    public MapView()
    {
        InitializeComponent();
    }

    private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Rectangle rect 
            && rect.DataContext is MapGridSubCellViewModel dc 
            && DataContext is MapViewModel vm)
        {
            vm.MouseOverItem = dc;
        }
    }

    private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Rectangle && DataContext is MapViewModel vm)
        {
            vm.MouseOverItem = null;
        }
    }

    private void AddGimmickButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MapViewModel viewModel)
        {
            var newGimmick = viewModel.AddGimmick();
            GimmickDataGrid.ScrollIntoView(newGimmick);
        }
    }

    private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Rectangle rect
            && rect.DataContext is MapGridSubCellViewModel dc
            && DataContext is MapViewModel vm)
        {
            vm.OnSubCellClicked(dc);
        }
    }
}
