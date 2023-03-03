using System.Windows;
using System.Windows.Controls;
using RanseiLink.ViewModels;

namespace RanseiLink.Views;

/// <summary>
/// Interaction logic for EvolutionTableView.xaml
/// </summary>
public partial class ScenarioWarriorWorkspaceView : UserControl
{
    public ScenarioWarriorWorkspaceView()
    {
        InitializeComponent();
    }
}

public class SwWorkspaceItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate WarriorTemplate { get; set; }
    public DataTemplate KingdomTemplate { get; set; }
    public DataTemplate KingdomSimpleTemplate { get; set; }

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
        return null;
    }
}
