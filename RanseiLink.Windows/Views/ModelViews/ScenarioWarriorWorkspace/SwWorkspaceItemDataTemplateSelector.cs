using System.Windows;
using System.Windows.Controls;
using RanseiLink.Windows.ViewModels;

namespace RanseiLink.Windows.Views;

public class SwWorkspaceItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate WarriorTemplate { get; set; }
    public DataTemplate KingdomTemplate { get; set; }
    public DataTemplate KingdomSimpleTemplate { get; set; }
    public DataTemplate ArmyTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is SwMiniViewModel)
        {
            return WarriorTemplate;
        }
        if (item is SwKingdomMiniViewModel)
        {
            return KingdomTemplate;
        }
        else if (item is SwSimpleKingdomMiniViewModel)
        {
            return KingdomSimpleTemplate;
        }
        else if (item is ScenarioArmyViewModel)
        {
            return ArmyTemplate;
        }
        return null;
    }
}
