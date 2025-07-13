using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using RanseiLink.GuiCore.Constants;

namespace RanseiLink.XP.Controls
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
            IconProperty.Changed.AddClassHandler<IconButton>(OnIconPropertyChanged);
        }

        private TextBlock _textBlock;


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _textBlock = e.NameScope.Find<TextBlock>(PART_IconTextBlock);

            if (_textBlock != null)
            {
                UpdateIcon(Icon);
            }
        }

        public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Icon));


        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static void OnIconPropertyChanged(IconButton d, AvaloniaPropertyChangedEventArgs e)
        {
            if (d is not IconButton c)
            {
                return;
            }
            c.UpdateIcon((string)e.NewValue);
        }

        private void UpdateIcon(string newIcon)
        {
            if (!Enum.TryParse<IconId>(newIcon, out var newIconEnum))
            {
                return;
            }
            if (_textBlock == null)
            {
                return;
            }
            _textBlock.Text = IconUtil.IconToStr(newIconEnum);
        }
    }
}
