using RanseiLink.Core.Settings;

namespace RanseiLink.Windows.Settings;

internal class RecentExportAnimFolderSetting : StringSetting
{
    public RecentExportAnimFolderSetting() : base("RecentExportAnimFolder")
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Export Anim";
        Description = "The path of the most recent folder exported animation to";
    }
}
