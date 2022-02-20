using RanseiLink.Core.Settings;
using RanseiLink.Settings;
using System;
using System.Windows;

namespace RanseiLink.Services.Concrete;

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
        var themeDictionary = Application.Current.Resources.MergedDictionaries[0];
        themeDictionary.MergedDictionaries.Clear();
        themeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"Styles/Colors/{CurrentTheme}.xaml", UriKind.RelativeOrAbsolute) });
    }
}
