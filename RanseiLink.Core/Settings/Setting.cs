using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace RanseiLink.Core.Settings
{
    public abstract class Setting<TSettingValue> : ISetting<TSettingValue>
    {
        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected bool RaiseAndSetIfChanged<T>(ref T property, T newValue, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, newValue))
            {
                property = newValue;
                RaisePropertyChanged(name);
                return true;
            }
            return false;
        }
        #endregion

        protected TSettingValue _default;
        private bool _isDefault;
        private TSettingValue _value;

        protected Setting(string uniqueElementName)
        {
            UniqueElementName = uniqueElementName;
        }

        public string UniqueElementName { get; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsHidden { get; protected set; }

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                if (RaiseAndSetIfChanged(ref _isDefault, value))
                {
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }

        public TSettingValue Value
        {
            get => _isDefault ? _default : _value;
            set
            {
                if (RaiseAndSetIfChanged(ref _value, value))
                {
                    _isDefault = false;
                    RaisePropertyChanged(nameof(IsDefault));
                }
            }
        }

        public abstract void Serialize(XElement element);
        public abstract void Deserialize(XElement element);

    }
}