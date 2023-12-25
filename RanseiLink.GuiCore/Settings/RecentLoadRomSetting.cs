using RanseiLink.Core.Settings;

namespace RanseiLink.GuiCore.Settings;

public class RecentLoadRomSetting : StringSetting
{
    public RecentLoadRomSetting() : base("RecentLoadRom") 
    {
        IsHidden = true;
        Name = "Recent Load Rom";
        Description = "The path of the most recent rom that was loaded";
    }
}
