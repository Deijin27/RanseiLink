using RanseiLink.Core.Settings;

namespace RanseiLink.Settings;

internal class RecentCommitRomSetting : StringSetting
{
    public RecentCommitRomSetting() : base("RecentCommitRom") 
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Patch Rom";
        Description = "The path of the most recent rom that was patched to";
    }
}
