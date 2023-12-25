using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Controls;
/// <summary>
/// Interaction logic for NumberButtonPanel.xaml
/// </summary>
public partial class NumberButtonPanel : UserControl
{
    public NumberButtonPanel()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty = UserControlUtil.RegisterDependencyProperty<NumberButtonPanel, int>(v => v.Value, default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty = UserControlUtil.RegisterDependencyProperty<NumberButtonPanel, string>(v => v.Items, OnItemsPropertyChanged);

    public string Items
    {
        get => (string)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    private static void OnItemsPropertyChanged(NumberButtonPanel target, DependencyPropertyChangedEventArgs<string> e)
    {
        List<int> values = new();
        var split = e.NewValue?.ToString()?.Split(',');
        if (split != null)
        {
            foreach (var item in split)
            {
                if (int.TryParse(item, out int value))
                {
                    values.Add(value);
                }
            }
        }
        target.ItemsControl.ItemsSource = values;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        if (btn.DataContext is int value)
        {
            Value = value;
        }
    }
}
