using System.Diagnostics;

namespace RanseiLink.GuiCore.Services;

public static class IssueReporter
{
    private const string _bug = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}";
    private const string _crash = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}&error={error}";

    private static void OpenReportUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = url
        };
        Process.Start(psi);
    }

    private static string Escape(string text)
    {
        return System.Net.WebUtility.UrlEncode(text);
    }

    public static void ReportBug(string appVersion)
    {
        OpenReportUrl(_bug
            .Replace("{version}", appVersion)
            );
    }

    public static void ReportCrash(string appVersion, string message)
    {
        OpenReportUrl(_crash
            .Replace("{version}", appVersion)
            .Replace("{error}", Escape(message))
            );
    }
}
