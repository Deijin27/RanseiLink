using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RanseiLink.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool RaiseAndSetIfChanged<T>(T currentValue, T newValue, Action<T> setter, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                setter(newValue);
                RaisePropertyChanged(name);
                return true;
            }
            return false;
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
    }
}
