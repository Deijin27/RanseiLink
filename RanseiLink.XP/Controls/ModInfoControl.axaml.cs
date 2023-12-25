using Avalonia;
using Avalonia.Controls;
using RanseiLink.Core.Services;

namespace RanseiLink.XP.Controls;
public partial class ModInfoControl : UserControl
{
    static ModInfoControl()
    {
        ModInfoProperty.Changed.AddClassHandler<ModInfoControl>(OnModInfoPropertyChanged);
    }

    public ModInfoControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<ModInfo> ModInfoProperty =
        AvaloniaProperty.Register<ModInfoControl, ModInfo>(nameof(ModInfo));

    public ModInfo ModInfo
    {
        get => GetValue(ModInfoProperty);
        set => SetValue(ModInfoProperty, value);
    }

    private static void OnModInfoPropertyChanged(ModInfoControl target, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not ModInfo mi)
        {
            target.NameTextBlock.Text = null;
            target.BottomTextBlock.Text = null;
        }
        else
        {
            target.NameTextBlock.Text = mi.Name;

            var bottomTextItems = new List<string>();
            if (!string.IsNullOrEmpty(mi.Version))
            {
                bottomTextItems.Add($"v{mi.Version}");
            }
            if (!string.IsNullOrEmpty(mi.Author))
            {
                bottomTextItems.Add(mi.Author);
            }
            bottomTextItems.Add(mi.GameCode.ToString());

            target.BottomTextBlock.Text = string.Join(" | ", bottomTextItems);

        }
    }
}
