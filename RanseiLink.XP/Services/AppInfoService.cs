#nullable enable
namespace RanseiLink.XP.Services;

public class AppInfoService : IAppInfoService
{
    public string Version => App.Version;
}
