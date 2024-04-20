using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RanseiLink.GuiCore.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notify to the view that the property <paramref name="name"/> has changed
    /// </summary>
    protected void Notify([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Notify to the view that all properties have changed
    /// </summary>
    protected void NotifyAll()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    /// <summary>
    /// If <paramref name="newValue"/> differs from the <paramref name="currentValue"/>, 
    /// sets the value and notifies the view of the change.
    /// </summary>
    protected bool Set<T>(T currentValue, T newValue, Action<T> setter, [CallerMemberName] string? name = null)
    {
        if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
        {
            setter(newValue);
            Notify(name);
            return true;
        }
        return false;
    }

    /// <summary>
    /// If <paramref name="newValue"/> differs from the current value of <paramref name="property"/>, 
    /// sets the value and notifies the view of the change.
    /// </summary>
    protected bool Set<T>(ref T property, T newValue, [CallerMemberName] string? name = null)
    {
        if (!EqualityComparer<T>.Default.Equals(property, newValue))
        {
            property = newValue;
            Notify(name);
            return true;
        }
        return false;
    }

}
