using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RanseiLink.Controls;

/// <summary>
/// Interaction logic for NumberBox.xaml
/// </summary>
public partial class NumberBox : UserControl
{
    public NumberBox()
    {
        InitializeComponent();
        NumberTextBox.Text = "0";
    }

    public static DependencyProperty ValueProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, int>(v => v.Value, default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuePropertyChanged);

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValuePropertyChanged(NumberBox target, DependencyPropertyChangedEventArgs<int> e)
    {
        target.NumberTextBox.Text = e.NewValue.ToString();
    }

    public static DependencyProperty MinProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, int>(v => v.Min, 0);

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static DependencyProperty MaxProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, int>(v => v.Max, int.MaxValue);

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static DependencyProperty IncrementProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, int>(v => v.Increment, 1);

    public int Increment
    {
        get => (int)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    private void IncrementButton_Click(object sender, RoutedEventArgs e)
    {
        int newVal = Value + Increment;
        if (newVal <= Max && newVal > Value)
        {
            Value = newVal;
            RaiseValueChanged();
        }
    }

    private void DecrementButton_Click(object sender, RoutedEventArgs e)
    {
        int newVal = Value - Increment;
        if (newVal >= Min && newVal < Value)
        {
            Value = newVal;
            RaiseValueChanged();
        }
    }

    private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var senderTextBox = (TextBox)sender;
        string text = senderTextBox.Text;
        var newVal = int.TryParse(text, out int i) ? i : Min;
        if (Value != newVal)
        {
            Value = newVal;
            RaiseValueChanged();
            if (string.IsNullOrEmpty(text))
            {
                senderTextBox.Text = "";
            }
        }
    }

    private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // If it's invlaid, mark as handled so it doesn't proceed, else mark as not handled.
        string newText = ((TextBox)sender).Text + e.Text;
        e.Handled = !(int.TryParse(newText, out int i) && i >= Min && i <= Max);
    }

    private void RaiseValueChanged()
    {
        ValueChanged?.Invoke(this, new EventArgs());
    }
    public event EventHandler ValueChanged;
}
