using RanseiLink.Core.Settings;

namespace RanseiLink.GuiCore.Settings;

public class RecentPatchRomSetting : StringSetting
{
    public RecentPatchRomSetting() : base("RecentCommitRom") 
    {
        IsHidden = true;
        Name = "Recent Patch Rom";
        Description = "The path of the most recent rom that was patched to";
    }
}
