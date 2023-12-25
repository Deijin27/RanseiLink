using Avalonia.Styling;
using RanseiLink.Core.Settings;

namespace RanseiLink.XP.Services;

public class RanseiLinkThemeVariant
{
    public static ThemeVariant Catppuccin_Mocha { get; } = new ThemeVariant("Catppuccin_Mocha", ThemeVariant.Dark);
}

internal class ThemeService : IThemeService
{
    private readonly ISettingService _settingsService;
    private readonly ThemeSetting _themeSetting;

    public ThemeService(ISettingService settingService)
    {
        _settingsService = settingService;
        _themeSetting = settingService.Get<ThemeSetting>();
        UpdateTheme();
    }

    public Theme CurrentTheme => _themeSetting.Value;

    public void SetTheme(Theme theme)
    {
        _themeSetting.Value = theme;
        _settingsService.Save();
        UpdateTheme();
    }

    private void UpdateTheme()
    {
        App.Current.RequestedThemeVariant = CurrentTheme switch
        {
            Theme.Dark => ThemeVariant.Dark,
            Theme.Light => ThemeVariant.Light,
            //Theme.Catppuccin_Mocha => RanseiLinkThemeVariant.Catppuccin_Mocha,
            _ => throw new NotImplementedException()
        };
    }
}