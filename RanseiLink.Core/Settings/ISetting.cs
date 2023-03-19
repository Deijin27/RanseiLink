using System.ComponentModel;
using System.Xml.Linq;

namespace RanseiLink.Core.Settings;

public interface ISetting
{
    bool IsHidden { get; }
    string UniqueElementName { get; }
    string Description { get; }
    bool IsDefault { get; set; }
    string Name { get; }
    void Deserialize(XElement element);
    void Serialize(XElement element);
}

public interface ISetting<TSettingValue> : ISetting, INotifyPropertyChanged
{
    TSettingValue Value { get; set; }
}