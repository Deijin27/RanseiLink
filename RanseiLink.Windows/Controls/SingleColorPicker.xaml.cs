using ColorPicker;
using ColorPicker.Models;
using RanseiLink.Core;
using System.Windows;

namespace RanseiLink.Windows.Controls;
/// <summary>
/// Interaction logic for PickerControl.xaml
/// </summary>
public partial class SingleColorPicker : PickerControlBase
{
    public SingleColorPicker()
    {
        InitializeComponent();

        PickerTypeComboBox.ItemsSource = EnumUtil.GetValues<PickerType>().ToArray();
    }

    public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(SingleColorPicker),
                new PropertyMetadata(1.0));

    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(SingleColorPicker),
            new PropertyMetadata(true));

    public static readonly DependencyProperty PickerTypeProperty
        = DependencyProperty.Register(nameof(PickerType), typeof(PickerType), typeof(SingleColorPicker),
            new PropertyMetadata(PickerType.HSV));

    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    public PickerType PickerType
    {
        get => (PickerType)GetValue(PickerTypeProperty);
        set => SetValue(PickerTypeProperty, value);
    }
}
