using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Views;

public class BuildingWorkspaceItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate BuildingTemplate { get; set; }
    public DataTemplate KingdomTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is BuildingViewModel)
        {
            return BuildingTemplate;
        }
        if (item is BuildingSimpleKingdomMiniViewModel)
        {
            return KingdomTemplate;
        }
        return null;
    }
}
