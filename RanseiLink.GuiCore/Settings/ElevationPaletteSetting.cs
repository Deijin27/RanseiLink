using RanseiLink.Core.Settings;
using RanseiLink.Core.Util;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public class ElevationPaletteSetting : Setting<IReadOnlyCollection<float>>
{
    public ElevationPaletteSetting() : base("ElevationPalette", [])
    {
        Name = "Elevation Palette";
        Description = "Values to be stored in elevation list";
    }

    public override void Deserialize(XElement element)
    {
        Value = element.Value.Split(';').Select(InvariantNumber.ParseFloat).ToArray();
    }

    public override void Serialize(XElement element)
    {
        element.Value = string.Join(';', Value.Select(InvariantNumber.FloatToString));
    }
}
