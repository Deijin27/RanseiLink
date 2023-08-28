using Avalonia.Controls;
using Avalonia.Styling;
using RanseiLink.Core.Settings;
using RanseiLink.XP.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.XP.Services;
public enum Theme
{
    Dark,
    Light,
    Catppuccin_Mocha
}

public class RanseiLinkThemeVariant
{
    public static ThemeVariant Catppuccin_Mocha { get; } = new ThemeVariant(nameof(Theme.Catppuccin_Mocha), ThemeVariant.Dark);
}

public interface IThemeService
{
    public Theme CurrentTheme { get; }
    public void SetTheme(Theme theme);
    public void ToggleTheme();
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

    public void ToggleTheme()
    {
        var newTheme = CurrentTheme switch
        {
            Theme.Dark => Theme.Light,
            Theme.Light => Theme.Dark,
            Theme.Catppuccin_Mocha => Theme.Dark,
            _ => Theme.Dark,
        };
        SetTheme(newTheme);
    }

    private void UpdateTheme()
    {
        App.Current.RequestedThemeVariant = CurrentTheme switch
        {
            Theme.Dark => ThemeVariant.Dark,
            Theme.Light => ThemeVariant.Light,
            Theme.Catppuccin_Mocha => RanseiLinkThemeVariant.Catppuccin_Mocha,
            _ => throw new NotImplementedException()
        };
    }
}