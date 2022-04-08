using RanseiLink.Core.Settings;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RanseiLink.Settings;

public class EditorModuleOrderSetting : Setting<IReadOnlyCollection<string>>
{
    public EditorModuleOrderSetting() : base("EditorModuleOrder") 
    {
        _default = Array.Empty<string>();
        Name = "Theme";
        Description = "The visual theme of the application";
    }

    public override void Deserialize(XElement element)
    {
        Value = element.Value.Split(',');
    }

    public override void Serialize(XElement element)
    {
        element.Value = string.Join(',', Value);
    }
}
