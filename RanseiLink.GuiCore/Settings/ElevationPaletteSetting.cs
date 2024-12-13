using RanseiLink.Core.Settings;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public class ElevationPaletteSetting : Setting<IReadOnlyCollection<string>>
{
    public ElevationPaletteSetting() : base("ElevationPalette", [])
    {
        Name = "Elevation Palette";
        Description = "Values to be stored in elevation list";
    }

    public override void Deserialize(XElement element)
    {
        Value = element.Value.Split(';');
    }

    public override void Serialize(XElement element)
    {
        element.Value = string.Join(';', Value);
    }
}
