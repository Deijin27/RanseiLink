using RanseiLink.Core.Settings;

namespace RanseiLink.Console.Settings;

internal class CurrentConsoleModSlotSetting : IntSetting
{
    public CurrentConsoleModSlotSetting() : base("CurrentConsoleModSlot")
    {
        _default = 0;
        Name = "Current Console Mod Slot";
        Description = "The index of the currently selected mod in the console command context";
    }
}
