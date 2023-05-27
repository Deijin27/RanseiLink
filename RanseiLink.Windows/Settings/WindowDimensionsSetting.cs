using System.Windows;
using RanseiLink.Core.Settings;
using System;
using System.Xml.Linq;

namespace RanseiLink.Windows.Settings;

public record WindowDimensions(double X, double Y, double Width, double Height, WindowState State);

public class WindowDimensionsSetting : Setting<WindowDimensions>
{
    public WindowDimensionsSetting() : base("WindowDimensions", new WindowDimensions(-1, -1, -1, -1, WindowState.Normal))
    {
    }

    public override void Deserialize(XElement element)
    {
        var xStr = element.Attribute("X")?.Value;
        var yStr = element.Attribute("Y")?.Value;
        var wStr = element.Attribute("Width")?.Value;
        var hStr = element.Attribute("Height")?.Value;
        var sStr = element.Attribute("State")?.Value;
        double.TryParse(xStr, out var x);
        double.TryParse(yStr, out var y);
        double.TryParse(wStr, out var width);
        double.TryParse(hStr, out var height);
        var state = Enum.TryParse<WindowState>(sStr, out var stateVal) ? stateVal : WindowState.Normal;
        Value = new WindowDimensions(x, y, width, height, state);
    }

    public override void Serialize(XElement element)
    {
        element.SetAttributeValue("X", Value.X);
        element.SetAttributeValue("Y", Value.Y);
        element.SetAttributeValue("Width", Value.Width);
        element.SetAttributeValue("Height", Value.Height);
        element.SetAttributeValue("State", Value.State);
    }
}
