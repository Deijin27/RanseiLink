using RanseiLink.ViewModels;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace RanseiLink.Views;

public partial class MapView : UserControl
{
    public MapView()
    {
        InitializeComponent();
    }

    private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Rectangle rect 
            && rect.DataContext is MapGridCellViewModel dc 
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
}
