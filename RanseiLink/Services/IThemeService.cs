
namespace RanseiLink.Services;

public enum Theme
{
    Dark,
    Light,
}

public interface IThemeService
{
    public Theme CurrentTheme { get; }
    public void SetTheme(Theme theme);
}
