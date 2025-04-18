using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class SelectorViewModel : ViewModelBase
{
    private int _selected;
    private readonly IModelService? _service;
    private object _nestedViewModel;
    private readonly Action<int> _onSelectedChanged;
    private readonly Func<int, bool> _validateId;

    public event EventHandler<int>? RequestNavigateToId;

    public SelectorViewModel(CopyPasteViewModel copyPasteVm, IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
    {
        _validateId = validateId;
        foreach (var item in displayItems)
        {
            DisplayItems.Add(item);
        }

        _onSelectedChanged = onSelectedChanged;
        _service = service;
        _nestedViewModel = nestedViewModel;
        _selected = DisplayItems.First().Id;
        UpdateNestedViewModel();
        RaisePropertyChanged(nameof(Selected));
        CopyPasteVisible = service != null && service.RetrieveObject(0) is IDataWrapper;
        CopyPasteVm = copyPasteVm;
        CopyPasteVm.ModelPasted += CopyPasteVm_ModelPasted;
        UpdateCopyPaste();
    }

    public object NestedViewModel
    {
        get => _nestedViewModel;
        set => SetProperty(ref _nestedViewModel, value);
    }


    public bool CopyPasteVisible { get; }

    
    public CopyPasteViewModel CopyPasteVm { get; }
    

    public void SetDisplayItems(List<SelectorComboBoxItem> displayItems)
    {
        DisplayItems.Clear();
        foreach (var item in displayItems)
        {
            DisplayItems.Add(item);
        }
        RaisePropertyChanged(nameof(Selected));
    }

    public ObservableCollection<SelectorComboBoxItem> DisplayItems { get; } = [];

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
                RequestNavigateToId?.Invoke(this, value);
            }
        }
    }

    public void SetSelected(int value)
    {
        if (!_validateId(value))
        {
            return;
        }
        if (_selected != value)
        {
            _selected = value;
            UpdateNestedViewModel();
            RaisePropertyChanged(nameof(Selected));
            UpdateCopyPaste();
        }
    }

    private void UpdateCopyPaste()
    {
        var currentModel = _service?.RetrieveObject(_selected);
        if (currentModel is not IDataWrapper modelDataWrapper)
        {
            return;
        }
        CopyPasteVm.Model = modelDataWrapper;
    }

    private void CopyPasteVm_ModelPasted(object? sender, EventArgs e)
    {
        if (!_validateId(Selected))
        {
            return;
        }
        UpdateNestedViewModel();
    }

    public void UpdateNestedViewModel()
    {
        _onSelectedChanged(_selected);
    }
}


// this is identical, except for the view side
public class SelectorViewModelWithoutScroll : SelectorViewModel
{
    public SelectorViewModelWithoutScroll(CopyPasteViewModel copyPasteVm, IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
        : base(copyPasteVm, service, displayItems, nestedViewModel, onSelectedChanged, validateId)
    {
    }
}