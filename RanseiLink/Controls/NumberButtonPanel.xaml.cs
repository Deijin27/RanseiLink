using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Controls;
/// <summary>
/// Interaction logic for NumberButtonPanel.xaml
/// </summary>
public partial class NumberButtonPanel : UserControl
{
    public NumberButtonPanel()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty = UserControlUtil.RegisterDependencyProperty<NumberButtonPanel, uint>(v => v.Value, default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

    public uint Value
    {
        get => (uint)GetValue(ValueProperty);
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
        List<uint> values = new();
        foreach (var item in e.NewValue.ToString().Split(','))
        {
            if (uint.TryParse(item, out uint value))
            {
                values.Add(value);
            }
        }
        target.ItemsControl.ItemsSource = values;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        if (btn.DataContext is uint value)
        {
            Value = value;
        }
    }
}
