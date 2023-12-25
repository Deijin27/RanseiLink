using Avalonia.Controls;
using RanseiLink.Core.Settings;

namespace RanseiLink.XP.Views;
public partial class MainWindow : Window
{
    private readonly ISettingService _settingService;

    public MainWindow()
    {
        _settingService = App.SettingService;
        InitializeComponent();
        Title = "RanseiLink XP " + App.Version;

        RestoreDimensions();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        SaveDimensions(_settingService);
        base.OnClosing(e);
    }

    private void RestoreDimensions()
    {
        var dims = _settingService.Get<WindowDimensionsSetting>().Value;
        if (dims.X > 0 && dims.Y > 0)
        {
            this.Position = new Avalonia.PixelPoint((int)dims.X, (int)dims.Y);
        }
        if (dims.Width > 0)
        {
            this.Width = dims.Width;
        }
        if (dims.Height > 0)
        {
            this.Height = dims.Height;
        }
        if (dims.State == GuiCore.Settings.WindowState.Normal)
        {
            this.WindowState = Avalonia.Controls.WindowState.Normal;
        }
        else if (dims.State == GuiCore.Settings.WindowState.Maximized)
        {
            this.WindowState = Avalonia.Controls.WindowState.Maximized;
        }
    }

    private void SaveDimensions(ISettingService settingService)
    {
        var dimSetting = settingService.Get<WindowDimensionsSetting>();
        var pos = this.Position;
        dimSetting.Value = new WindowDimensions(pos.X, pos.Y, Width, Height, 
            WindowState == Avalonia.Controls.WindowState.Maximized ? GuiCore.Settings.WindowState.Maximized : GuiCore.Settings.WindowState.Normal);
        settingService.Save();
    }
}
