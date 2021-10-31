using RanseiLink.Core.Services;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Controls
{
    /// <summary>
    /// Interaction logic for ModListItemView.xaml
    /// </summary>
    public partial class ModInfoControl : UserControl
    {
        public ModInfoControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty ModInfoProperty = UserControlUtil.RegisterDependencyProperty<ModInfoControl, ModInfo>(
            v => v.ModInfo, new ModInfo(), OnModInfoPropertyChanged);

        public ModInfo ModInfo
        {
            get => (ModInfo)GetValue(ModInfoProperty);
            set => SetValue(ModInfoProperty, value);
        }

        private static void OnModInfoPropertyChanged(ModInfoControl target, DependencyPropertyChangedEventArgs<ModInfo> e)
        {
            var mi = e.NewValue;
            if (e.NewValue != null)
            {
                target.NameTextBlock.Text = mi.Name;
                target.AuthorTextBlock.Text = $"by {mi.Author}";
                target.VersionTextBlock.Text = $"v{mi.Version}";
            }
        }
    }
}
