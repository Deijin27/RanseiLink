using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System.Windows;

namespace RanseiLink.Windows.Dialogs;

/// <summary>
/// Interaction logic for PluginDialog.xaml
/// </summary>
public partial class PluginDialog : Window
{
    public PluginDialog()
    {
        InitializeComponent();
    }

    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();

    }
}


public class TestPluginFormInfo
{
    public class PluginFormTest : IPluginForm
    {
        public string Title => "Test Title";

        public string ProceedButtonText => "Proceed";

        public string CancelButtonText => "Don't proceed";
    }

    public IPluginForm Form { get; } = new PluginFormTest();
    public List<IPluginFormItem> Items { get; } = 
    [
        new BoolPluginFormItem(null, "Battle Maps", null, false),
        new BoolPluginFormItem(null, "Battle Maps", "Randomly choose a battle config for each kingdom and each battle building", true),
        new HeaderPluginFormItem("Header"),
        new IntPluginFormItem(null, "Minimum max link value", "Set max link to at least this value", 98, 0, 100),
        new StringPluginFormItem(null, "Randomization Seed", "Seed used for the randomizer. Using the same seed will give the same result", "hello", int.MaxValue),
       
    ];
}