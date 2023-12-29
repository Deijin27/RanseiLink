namespace RanseiLink.Windows.Services.Concrete;
public class AppInfoService : IAppInfoService
{
    public string Version => App.Version;
}
