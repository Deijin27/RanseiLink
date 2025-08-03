
using RanseiLink.GuiCore.Constants;

namespace RanseiLink.GuiCore.Services;

public enum Theme
{
    Dark,
    Light,
}

public interface IThemeService
{
    public Theme CurrentTheme { get; }
    public void SetTheme(Theme theme);

    public void ToggleTheme()
    {
        var newTheme = CurrentTheme switch
        {
            Theme.Dark => Theme.Light,
            Theme.Light => Theme.Dark,
            _ => Theme.Dark,
        };
        SetTheme(newTheme);
    }

    public IconId ThemeIcon()
    {
        return CurrentTheme switch
        {
            Theme.Light => IconId.light_mode,
            Theme.Dark => IconId.dark_mode,
            _ => IconId.light_mode,
        };
    }
}
