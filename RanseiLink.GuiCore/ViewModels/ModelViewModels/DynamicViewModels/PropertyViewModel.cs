using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.GuiCore.ViewModels;


public class PropertyCollectionViewModel
{
    public ObservableCollection<PropertyViewModel> Properties { get; } = [];
}

public class PropertyViewModel : ViewModelBase
{
    public PropertyViewModel(string displayName)
    {
        DisplayName = displayName;
    }
    public string DisplayName { get; }
}

public class PropertyViewModel<T> : PropertyViewModel
{
    private readonly Func<T> _getValue;
    private readonly Action<T> _setValue;

    public PropertyViewModel(string displayName, Func<T> getValue, Action<T> setValue) : base(displayName)
    {
        _getValue = getValue;
        _setValue = setValue;
    }

    public T Value
    {
        get => _getValue();
        set => SetProperty(Value, value, _setValue);
    }
}

public class StringPropertyViewModel : PropertyViewModel<string>
{
    public StringPropertyViewModel(string displayName, Func<string> getValue, Action<string> setValue, int maxLength)
        : base(displayName, getValue, setValue)
    {
        MaxLength = maxLength;
    }

    public int MaxLength { get; }
}

public class IntPropertyViewModel : PropertyViewModel<int>
{
    public IntPropertyViewModel(string displayName, Func<int> getValue, Action<int> setValue, int min = 0, int max = int.MaxValue)
        : base(displayName, getValue, setValue)
    {
        Min = min;
        Max = max;
    }

    public int Min { get; }
    public int Max { get; }

}

public class ComboPropertyViewModel : PropertyViewModel<int>
{
    public ComboPropertyViewModel(string displayName, Func<int> getValue, Action<int> setValue, IReadOnlyCollection<SelectorComboBoxItem> items)
        : base(displayName, getValue, setValue)
    {
        Items = items;
    }

    public IReadOnlyCollection<SelectorComboBoxItem> Items { get; }
}
