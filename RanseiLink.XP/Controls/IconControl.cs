using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using RanseiLink.GuiCore.Constants;

namespace RanseiLink.XP.Controls
{
    [TemplatePart(Name = PART_IconTextBlock, Type = typeof(TextBlock))]
    public class IconControl : TemplatedControl
    {
        public const string PART_IconTextBlock = nameof(PART_IconTextBlock);
        public IconControl()
        {
        }

        static IconControl()
        {
            IconProperty.Changed.AddClassHandler<IconControl>(OnIconPropertyChanged);
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
        AvaloniaProperty.Register<IconControl, string>(nameof(Icon));

        public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<IconControl, double>(
           nameof(Size),
           defaultValue: 15.0
           );


        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        private static void OnIconPropertyChanged(IconControl d, AvaloniaPropertyChangedEventArgs e)
        {
             d?.UpdateIcon((string)e.NewValue);
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
