using RanseiLink.Core.Settings;

namespace RanseiLink.GuiCore.Settings;

public class RecentExportModFolderSetting : StringSetting
{
    public RecentExportModFolderSetting() : base("RecentExportFolder") 
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Export Mod";
        Description = "The path of the most recent folder exported mod to";
    }
}
