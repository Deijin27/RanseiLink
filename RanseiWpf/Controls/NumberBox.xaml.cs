using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RanseiWpf.Controls
{
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

        public static DependencyProperty ValueProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Value, default, OnValuePropertyChanged);

        public uint Value
        {
            get => (uint)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValuePropertyChanged(NumberBox target, DependencyPropertyChangedEventArgs<uint> e)
        {
            target.NumberTextBox.Text = e.NewValue.ToString();
        }

        public static DependencyProperty MinProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Min, uint.MinValue);

        public uint Min
        {
            get => (uint)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public static DependencyProperty MaxProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Max, uint.MaxValue);

        public uint Max
        {
            get => (uint)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static DependencyProperty IncrementProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Increment, 1u);

        public uint Increment
        {
            get => (uint)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            uint newVal = Value + Increment;
            if (newVal <= Max && newVal > Value)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            uint newVal = Value - Increment;
            if (newVal >= Min && newVal < Value)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            var newVal = uint.TryParse(text, out uint i) ? i : Min;
            if (Value != newVal)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // If it's invlaid, mark as handled so it doesn't proceed, else mark as not handled.
            string newText = ((TextBox)sender).Text + e.Text;
            e.Handled = !(uint.TryParse(newText, out uint i) && i >= Min && i <= Max);
        }

        private void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }
        public event EventHandler ValueChanged;
    }
}
