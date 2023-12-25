using RanseiLink.Core.Settings;

namespace RanseiLink.GuiCore.Settings;

public class RecentExportAnimFolderSetting : StringSetting
{
    public RecentExportAnimFolderSetting() : base("RecentExportAnimFolder")
    {
        IsHidden = true;
        Name = "Recent Export Anim";
        Description = "The path of the most recent folder exported animation to";
    }
}
