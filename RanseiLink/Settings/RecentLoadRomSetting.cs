using RanseiLink.Core.Settings;

namespace RanseiLink.Settings;

internal class RecentLoadRomSetting : StringSetting
{
    public RecentLoadRomSetting() : base("RecentLoadRom") 
    {
        _default = null;
        IsHidden = true;
        Name = "Recent Load Rom";
        Description = "The path of the most recent rom that was loaded";
    }
}
