
namespace RanseiLink.Core.Services;

public interface ISettingsService
{
    int CurrentConsoleModSlot { get; set; }
    string RecentCommitRom { get; set; }
    string RecentLoadRom { get; set; }
    string RecentExportModFolder { get; set; }
}
