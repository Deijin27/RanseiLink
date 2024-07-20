using RanseiLink.Core.Settings;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public class PinnedModsSetting : Setting<IReadOnlyCollection<string>>
{
    public PinnedModsSetting() : base("PinnedMods", Array.Empty<string>())
    {
        _default = Array.Empty<string>();
        Name = "Pinned mods";
        Description = "The Folder of each pinned mod";
    }

    public override void Deserialize(XElement element)
    {
        Value = element.Elements("Item").Select(x => x.Value).ToArray();
    }

    public override void Serialize(XElement element)
    {
        element.RemoveNodes();
        foreach (var item in Value)
        {
            element.Add(new XElement("Item", item));
        }
    }
}
