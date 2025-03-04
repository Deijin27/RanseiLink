﻿using RanseiLink.Core.Settings;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public class EditorModuleOrderSetting : Setting<IReadOnlyCollection<string>>
{
    public EditorModuleOrderSetting() : base("EditorModuleOrder", Array.Empty<string>()) 
    {
        _default = Array.Empty<string>();
        Name = "Editor module order";
        Description = "The order of the tab items in the list at the side of the main editor page";
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