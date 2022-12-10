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

public class ScenarioWarriorWorkspaceItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate WarriorTemplate { get; set; }
    public DataTemplate KingdomTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ScenarioWarriorMiniViewModel)
        {
            return WarriorTemplate;
        }
        if (item is ScenarioWarriorKingdomMiniViewModel)
        {
            return KingdomTemplate;
        }
        return null;
    }
}
