using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.XP.Services;
public class IssueReporter
{
    private const string c_bug = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}";
    private const string c_crash = "https://github.com/Deijin27/RanseiLink/issues/new?template=bug_report.yml&version={version}&error={error}";

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

    public static void ReportBug()
    {
        OpenReportUrl(c_bug
            .Replace("{version}", App.Version)
            );
    }

    public static void ReportCrash(string message)
    {
        OpenReportUrl(c_crash
            .Replace("{version}", App.Version)
            .Replace("{error}", Escape(message))
            );
    }
}
