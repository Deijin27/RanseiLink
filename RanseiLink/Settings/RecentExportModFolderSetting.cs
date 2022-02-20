using RanseiLink.Core.Settings;

namespace RanseiLink.Settings;

internal class RecentExportModFolderSetting : StringSetting
{
    public RecentExportModFolderSetting() : base("RecentExportFolder") 
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Patch Rom";
        Description = "The path of the most recent rom that was patched to";
    }
}
