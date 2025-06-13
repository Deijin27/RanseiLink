using System.Windows;

namespace RanseiLink.Windows.Behaviours;
public static class ButtonBehaviours
{
    public static readonly DependencyProperty IsDangerousProperty =
        DependencyProperty.RegisterAttached("IsDangerous", typeof(bool), typeof(ButtonBehaviours));

    public static bool GetIsDangerous(UIElement target)
    {
        return (bool)target.GetValue(IsDangerousProperty);
    }

    public static void SetIsDangerous(UIElement target, bool value)
    {
        target.SetValue(IsDangerousProperty, value);
    }
}
