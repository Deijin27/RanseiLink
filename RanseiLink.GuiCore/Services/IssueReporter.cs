namespace RanseiLink.GuiCore.Services;

public static class IssueReporter
{
    private const string _bug = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}";
    private const string _crash = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}&error={error}";

    

    private static string Escape(string text)
    {
        return System.Net.WebUtility.UrlEncode(text);
    }

    public static void ReportBug(string appVersion)
    {
        WebUtil.OpenUrl(_bug
            .Replace("{version}", appVersion)
            );
    }

    public static void ReportCrash(string appVersion, string message)
    {
        WebUtil.OpenUrl(_crash
            .Replace("{version}", appVersion)
            .Replace("{error}", Escape(message))
            );
    }
}
