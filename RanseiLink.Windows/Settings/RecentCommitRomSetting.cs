using RanseiLink.Core.Settings;

namespace RanseiLink.Windows.Settings;

public class RecentCommitRomSetting : StringSetting
{
    public RecentCommitRomSetting() : base("RecentCommitRom") 
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Patch Rom";
        Description = "The path of the most recent rom that was patched to";
    }
}
