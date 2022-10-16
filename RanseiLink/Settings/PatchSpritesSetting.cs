using RanseiLink.Core.Settings;

namespace RanseiLink.Settings;

public class PatchSpritesSetting : BoolSetting
{
    public PatchSpritesSetting() : base("PatchSprites") 
    {
        _default = true;
        Name = "Patch Sprites";
        Description = "Include sprites in patch";
    }
}
