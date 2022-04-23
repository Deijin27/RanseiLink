using RanseiLink.Core.Services.ModelServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public class SelectorViewModel : ViewModelBase
{
    private int _selected;
    private object _nestedViewModel;
    private readonly Action<int> _onSelectedChanged;
    private List<SelectorComboBoxItem> _displayItems;
    private readonly Func<int, bool> _validateId;

    public SelectorViewModel(IModelService service, object nestedViewModel, Action<int> onSelectedChanged)
        :this(service.GetComboBoxItemsExceptDefault(), nestedViewModel, onSelectedChanged, service.ValidateId)
    {
    }

    public SelectorViewModel(List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
    {
        _validateId = validateId;
        DisplayItems = displayItems;

        _onSelectedChanged = onSelectedChanged;
        NestedViewModel = nestedViewModel;
        _selected = DisplayItems.First().Id;
        UpdateNestedViewModel();
        RaisePropertyChanged(nameof(Selected));
    }

    public object NestedViewModel
    {
        get => _nestedViewModel;
        set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
    }

    public List<SelectorComboBoxItem> DisplayItems
    {
        get => _displayItems;
        set => RaiseAndSetIfChanged(ref _displayItems, value);
    }

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