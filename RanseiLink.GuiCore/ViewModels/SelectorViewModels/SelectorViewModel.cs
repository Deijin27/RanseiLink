using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class SelectorViewModel : ViewModelBase
{
    private int _selected;
    private object _nestedViewModel;
    private readonly Action<int> _onSelectedChanged;
    private readonly Func<int, bool> _validateId;

    public SelectorViewModel(IModelService service, object nestedViewModel, Action<int> onSelectedChanged)
        :this(service.GetComboBoxItemsExceptDefault(), nestedViewModel, onSelectedChanged, service.ValidateId)
    {
    }

    public SelectorViewModel(List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
    {
        _validateId = validateId;
        foreach (var item in displayItems)
        {
            DisplayItems.Add(item);
        }

        _onSelectedChanged = onSelectedChanged;
        _nestedViewModel = nestedViewModel;
        _selected = DisplayItems.First().Id;
        UpdateNestedViewModel();
        RaisePropertyChanged(nameof(Selected));
    }

    public object NestedViewModel
    {
        get => _nestedViewModel;
        set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
    }

    public void SetDisplayItems(List<SelectorComboBoxItem> displayItems)
    {
        DisplayItems.Clear();
        foreach (var item in displayItems)
        {
            DisplayItems.Add(item);
        }
        RaisePropertyChanged(nameof(Selected));
    }

    public ObservableCollection<SelectorComboBoxItem> DisplayItems { get; } = new ObservableCollection<SelectorComboBoxItem>();

    public int Selected
    {
        get => _selected;
        set
        {
            if (!_validateId(value))
            {
                return;
            }
            if (_selected != value)
            {
                _selected = value;
                UpdateNestedViewModel();
                RaisePropertyChanged();
            }
        }
    }

    public void UpdateNestedViewModel()
    {
        _onSelectedChanged(_selected);
    }
}


// this is identical, except for the view side
public class SelectorViewModelWithoutScroll : SelectorViewModel
{
    public SelectorViewModelWithoutScroll(IModelService service, object nestedViewModel, Action<int> onSelectedChanged)
        : base(service, nestedViewModel, onSelectedChanged) { }

    public SelectorViewModelWithoutScroll(List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
        : base(displayItems, nestedViewModel, onSelectedChanged, validateId)
    {
    }
}