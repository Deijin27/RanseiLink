using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class SelectorViewModel : ViewModelBase
{
    private int _selected;
    private readonly IAsyncDialogService _dialogService;
    private readonly IClipboardService _clipboardService;
    private readonly IModelService? _service;
    private object _nestedViewModel;
    private readonly Action<int> _onSelectedChanged;
    private readonly Func<int, bool> _validateId;

    public SelectorViewModel(IAsyncDialogService dialogService, IClipboardService clipboardService, IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
    {
        _validateId = validateId;
        foreach (var item in displayItems)
        {
            DisplayItems.Add(item);
        }

        _onSelectedChanged = onSelectedChanged;
        _dialogService = dialogService;
        _clipboardService = clipboardService;
        _service = service;
        _nestedViewModel = nestedViewModel;
        _selected = DisplayItems.First().Id;
        UpdateNestedViewModel();
        RaisePropertyChanged(nameof(Selected));
        CopyPasteVisible = service != null && service.RetrieveObject(0) is IDataWrapper;
        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
    }

    public object NestedViewModel
    {
        get => _nestedViewModel;
        set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
    }


    public bool CopyPasteVisible { get; }

    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }

    private async void Copy()
    {
        var currentModel = _service?.RetrieveObject(_selected);
        if (currentModel is not IDataWrapper modelDataWrapper)
        {
            return;
        }
        var text = modelDataWrapper.Serialize();
        var success = await _clipboardService.SetText(text);
        if (success)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Copied", "Data of this entry is copied to your clipboard"));
        }
        else
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Could not Copy", "Failed to set clipboard text", MessageBoxType.Error));
        }
    }

    private async void Paste()
    {
        var currentModel = _service?.RetrieveObject(_selected);
        if (currentModel is not IDataWrapper modelDataWrapper)
        {
            return;
        }
        var text = await _clipboardService.GetText();
        if (text == null)
        {
            return;
        }
        var result = await _dialogService.ShowMessageBox(new MessageBoxSettings(
            "Paste Data",
            "You want to replace this entry's data with the clipboard contents?",
            [
                new("Yes", MessageBoxResult.Yes),
                new("Cancel", MessageBoxResult.No)
            ]
            ));
        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        if (!modelDataWrapper.TryDeserialize(text))
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Could not Paste", "Failed to load data from clipboard", MessageBoxType.Error));
            return;
        }

        if (!_validateId(Selected))
        {
            return;
        }
        UpdateNestedViewModel();
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
    public SelectorViewModelWithoutScroll(IAsyncDialogService dialogService, IClipboardService clipboardService, IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId)
        : base(dialogService, clipboardService, service, displayItems, nestedViewModel, onSelectedChanged, validateId)
    {
    }
}