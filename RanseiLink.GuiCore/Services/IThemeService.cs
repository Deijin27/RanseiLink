
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
}
