using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RanseiLink.Views;

internal static class DataGridHelper
{
    public static ScrollViewer FindScrollViewer(DependencyObject d)
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
