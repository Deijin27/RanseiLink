using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RanseiLink.XP.Controls;
public partial class IconButton : UserControl
{
    static IconButton()
    {
        IconProperty.Changed.AddClassHandler<IconButton>(OnIconPropertyChanged);
        TextProperty.Changed.AddClassHandler<IconButton>(OnTextPropertyChanged);
        CommandProperty.Changed.AddClassHandler<IconButton>(OnCommandPropertyChanged);
        CommandParameterProperty.Changed.AddClassHandler<IconButton>(OnCommandParameterPropertyChanged);
    }

    public IconButton()
    {
        InitializeComponent();
        IconTextBlock.Text = Icon;
        TextTextBlock.Text = Text;
    }

    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Icon));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Text));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<IconButton, ICommand>(nameof(Command));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<IconButton, object>(nameof(CommandParameter));

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public RoutedEvent Click;

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        //Click?.(this, e);
    }

    public static void OnIconPropertyChanged(IconButton d, AvaloniaPropertyChangedEventArgs e)
    {
        if (d is not IconButton c)
        {
            return;
        }
        c.IconTextBlock.Text = e.NewValue as string;
    }

    public static void OnTextPropertyChanged(IconButton d, AvaloniaPropertyChangedEventArgs e)
    {
        if (d is not IconButton c)
        {
            return;
        }
        var newValue = e.NewValue as string;
        c.TextTextBlock.Text = newValue;
        if (string.IsNullOrEmpty(newValue))
        {
            c.TextTextBlock.IsVisible = false;
        }
        else
        {
            c.TextTextBlock.IsVisible = true;
        }

    }

    public static void OnCommandPropertyChanged(IconButton d, AvaloniaPropertyChangedEventArgs e)
    {
        if (d is not IconButton c)
        {
            return;
        }
        c.MainButton.Command = e.NewValue as ICommand;
    }

    public static void OnCommandParameterPropertyChanged(IconButton d, AvaloniaPropertyChangedEventArgs e)
    {
        if (d is not IconButton c)
        {
            return;
        }
        c.MainButton.CommandParameter = e.NewValue;
    }
}
