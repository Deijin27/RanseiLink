using System.Xml.Linq;

namespace RanseiLink.Core.Settings
{
    public abstract class BoolSetting : Setting<bool>
    {
        protected BoolSetting(string uniqueElementName) : base(uniqueElementName, default) { }

        public override void Deserialize(XElement element)
        {
            if (bool.TryParse(element.Value, out bool value))
            {
                Value = value;
            }
        }

        public override void Serialize(XElement element)
        {
            element.Value = Value.ToString();
        }
    }
}