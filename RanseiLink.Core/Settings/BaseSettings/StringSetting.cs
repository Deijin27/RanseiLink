using System.Xml.Linq;

namespace RanseiLink.Core.Settings;

public abstract class StringSetting : Setting<string>
{
    protected StringSetting(string uniqueElementName) : base(uniqueElementName, string.Empty) { }

    public override void Deserialize(XElement element)
    {
        Value = element.Value;
    }

    public override void Serialize(XElement element)
    {
        element.Value = Value;
    }
}