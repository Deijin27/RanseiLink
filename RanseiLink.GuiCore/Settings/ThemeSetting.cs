using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.Services;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public class ThemeSetting : Setting<Theme>
{
    public ThemeSetting() : base("Theme", Theme.Dark) 
    {
        _default = Theme.Dark;
        Name = "Theme";
        Description = "The visual theme of the application";
    }

    public override void Deserialize(XElement element)
    {
        if (Enum.TryParse<Theme>(element.Value, out var theme))
        {
            Value = theme;
        }
    }

    public override void Serialize(XElement element)
    {
        element.Value = Value.ToString();
    }
}
