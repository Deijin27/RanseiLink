using RanseiLink.GuiCore.Constants;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RanseiLink.Windows.Controls
{
    [TemplatePart(Name = PART_IconTextBlock, Type = typeof(TextBlock))]
    public class IconButton : Button
    {
        public const string PART_IconTextBlock = nameof(PART_IconTextBlock);
        public IconButton()
        {
        }

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        private TextBlock _textBlock;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBlock = GetTemplateChild(PART_IconTextBlock) as TextBlock;

            if (_textBlock != null)
            {
                UpdateIcon(Icon);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(IconId),
            typeof(IconButton),
            new PropertyMetadata(IconId.category, OnIconPropertyChanged)
            );


        public IconId Icon
        {
            get => (IconId)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
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
