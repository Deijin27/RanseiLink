using RanseiLink.Core.Settings;
using RanseiLink.Core.Util;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Settings;

public enum WindowState
{
    Normal,
    Maximized
}

public record WindowDimensions(double X, double Y, double Width, double Height, WindowState State);

public class WindowDimensionsSetting : Setting<WindowDimensions>
{
    public WindowDimensionsSetting() : base("WindowDimensions", new WindowDimensions(-1, -1, -1, -1, WindowState.Normal))
    {
    }

    public override void Deserialize(XElement element)
    {
        double x = -1;
        double y = -1;
        double w = -1;
        double h = -1;
        WindowState s = WindowState.Normal;

        var xAttr = element.Attribute("X");
        var yAttr = element.Attribute("Y");
        var wAttr = element.Attribute("Width");
        var hAttr = element.Attribute("Height");
        var sAttr = element.Attribute("State");

        var max = short.MaxValue;

        if (xAttr != null && InvariantNumber.TryParseDouble(xAttr.Value, out var xParsed) && xParsed < max)
        {
            x = xParsed;
        }

        if (yAttr != null && InvariantNumber.TryParseDouble(yAttr.Value, out var yParsed) && yParsed < max)
        {
            y = yParsed;
        }

        if (wAttr != null && InvariantNumber.TryParseDouble(wAttr.Value, out var wParsed) && wParsed < max)
        {
            w = wParsed;
        }

        if (hAttr != null && InvariantNumber.TryParseDouble(hAttr.Value, out var hParsed) && hParsed < max)
        {
            h = hParsed;
        }

        if (sAttr != null && Enum.TryParse<WindowState>(sAttr.Value, out var sParsed))
        {
            s = sParsed;
        }

        Value = new WindowDimensions(x, y, w, h, s);
    }

    public override void Serialize(XElement element)
    {
        element.SetAttributeValue("X", InvariantNumber.DoubleToString(Value.X));
        element.SetAttributeValue("Y", InvariantNumber.DoubleToString(Value.Y));
        element.SetAttributeValue("Width", InvariantNumber.DoubleToString(Value.Width));
        element.SetAttributeValue("Height", InvariantNumber.DoubleToString(Value.Height));
        element.SetAttributeValue("State", Value.State);
    }
}
