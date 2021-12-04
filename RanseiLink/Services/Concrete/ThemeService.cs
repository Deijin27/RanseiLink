using RanseiLink.Core.Services;
using System;
using System.Windows;

namespace RanseiLink.Services.Concrete;

internal class ThemeService : IThemeService
{
    private readonly ISettingsService _settingsService;

    public ThemeService(ISettingsService settingService)
    {
        _settingsService = settingService;
        CurrentTheme = Enum.Parse<Theme>(_settingsService.Theme);
        UpdateTheme();
    }

    public Theme CurrentTheme { get; set; }

    public void SetTheme(Theme theme)
    {
        _settingsService.Theme = theme.ToString();
        CurrentTheme = theme;
        UpdateTheme();
    }

    private void UpdateTheme()
    {
        var themeDictionary = Application.Current.Resources.MergedDictionaries[0];
        themeDictionary.MergedDictionaries.Clear();
        themeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"Styles/Colors/{CurrentTheme}.xaml", UriKind.RelativeOrAbsolute) });
    }
}
