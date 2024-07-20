using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Controls
{
    [TemplatePart(Name = PART_TextBlock, Type = typeof(TextBlock))]
    public class IconControl : Control
    {
        public const string PART_TextBlock = nameof(PART_TextBlock);

        static IconControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconControl), new FrameworkPropertyMetadata(typeof(IconControl)));
        }

        private TextBlock _textBlock;

        public IconControl()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBlock = GetTemplateChild(PART_TextBlock) as TextBlock;

            if (_textBlock != null)
            {
                UpdateIcon(Icon);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(IconId),
            typeof(IconControl),
            new PropertyMetadata(IconId.category, OnIconPropertyChanged)
            );

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(IconControl),
            new PropertyMetadata(15.0)
            );

        public IconId Icon
        {
            get => (IconId)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconControl c)
            {
                return;
            }
            c.UpdateIcon((IconId)e.NewValue);
        }

        private void UpdateIcon(IconId newIcon)
        {
            if (_textBlock == null)
            {
                return;
            }
            _textBlock.Text = IconUtil.IconToStr(newIcon);
        }
    }
}
