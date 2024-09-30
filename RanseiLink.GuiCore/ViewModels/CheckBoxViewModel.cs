namespace RanseiLink.GuiCore.ViewModels;

public class CheckBoxViewModel : ViewModelBase
{
    private readonly Func<bool> _getValue;
    private readonly Action<bool> _setValue;

    public CheckBoxViewModel(string name, Func<bool> getValue, Action<bool> setValue)
    {
        Name = name;
        _getValue = getValue;
        _setValue = setValue;
    }

    public string Name { get; }
    public bool IsChecked
    {
        get => _getValue();
        set
        {
            if (value != IsChecked)
            {
                _setValue(value);
                RaisePropertyChanged();
            }
        }
    }
}
