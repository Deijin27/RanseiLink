using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Controls
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
            IconTextBlock.Text = Icon;
            TextTextBlock.Text = Text;
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata("\xe88b", OnIconPropertyChanged)
            );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata("", OnTextPropertyChanged)
            );

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(IconButton),
            new PropertyMetadata(null, OnCommandPropertyChanged)
            );

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(IconButton),
            new PropertyMetadata(null, OnCommandParameterPropertyChanged)
            );

        public static readonly RoutedEvent ConditionalClickEvent = EventManager.RegisterRoutedEvent(
            nameof(Click),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(IconButton)
            );

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }


        public event RoutedEventHandler Click;

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
            {
                return;
            }
            c.IconTextBlock.Text = e.NewValue as string;
        }

        public static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
            {
                return;
            }
            var newValue = e.NewValue as string;
            c.TextTextBlock.Text = newValue;
            if (string.IsNullOrEmpty(newValue))
            {
                c.TextTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                c.TextTextBlock.Visibility = Visibility.Visible;
            }

        }

        public static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
            {
                return;
            }
            c.MainButton.Command = e.NewValue as ICommand;
        }

        public static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
            {
                return;
            }
            c.MainButton.CommandParameter = e.NewValue;
        }
    }
}
