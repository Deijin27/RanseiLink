using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Controls;

public class OverrideControl : Control
{
    static OverrideControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(OverrideControl), new FrameworkPropertyMetadata(typeof(OverrideControl)));
    }

    public static readonly DependencyProperty IsOverrideProperty = DependencyProperty.Register(
        nameof(IsOverride),
        typeof(bool),
        typeof(OverrideControl)
        );


    public bool IsOverride
    {
        get => (bool)GetValue(IsOverrideProperty);
        set => SetValue(IsOverrideProperty, value);
    }
}
