using Avalonia.Controls;
using RanseiLink.Core.Settings;
using RanseiLink.XP.Settings;

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
        if (dims.State == WindowState.Normal || dims.State == WindowState.Maximized)
        {
            this.WindowState = dims.State;
        }
    }

    private void SaveDimensions(ISettingService settingService)
    {
        var dimSetting = settingService.Get<WindowDimensionsSetting>();
        var pos = this.Position;
        dimSetting.Value = new WindowDimensions(pos.X, pos.Y, Width, Height, WindowState);
        settingService.Save();
    }
}
