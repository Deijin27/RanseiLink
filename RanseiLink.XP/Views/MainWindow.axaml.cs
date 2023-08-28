using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Reflection;

namespace RanseiLink.XP.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = "RanseiLink XP " + App.Version;
    }
}
