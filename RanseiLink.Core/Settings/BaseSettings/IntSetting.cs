using System.Xml.Linq;

namespace RanseiLink.Core.Settings
{
    public abstract class IntSetting : Setting<int>
    {
        protected IntSetting(string uniqueElementName) : base(uniqueElementName, default) { }

        public override void Deserialize(XElement element)
        {
            if (int.TryParse(element.Value, out int value))
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