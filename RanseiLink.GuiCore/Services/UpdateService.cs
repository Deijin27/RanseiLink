using System.Text.Json;

namespace RanseiLink.GuiCore.Services;

public interface IUpdateService
{
    Task<bool> IsUpdateAvailable();
    void OpenDownloadPage();
}

internal class UpdateService(AppInfo appInfo) : IUpdateService
{
    public async Task<bool> IsUpdateAvailable()
    {
        var latestVersion = await GetGithubRelease();
        if (latestVersion == null)
        {
            return false;
        }
        return latestVersion.IsNewerThan(appInfo.Version);
    }

    public void OpenDownloadPage()
    {
        var url = "https://github.com/Deijin27/RanseiLink/releases/latest";
        WebUtil.OpenUrl(url);
    }

    private static async Task<AppVersion?> GetGithubRelease()
    {
        try
        {
            var url = "https://api.github.com/repos/Deijin27/RanseiLink/releases/latest";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "RanseiLink");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var version = json.RootElement.GetProperty("tag_name").GetString();
            return AppVersion.Parse(version);
        }
        catch
        {
            return null;
        }
    }    
}
