using RanseiLink.Core;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase, ISaveableRefreshable
    where TModel : IDataWrapper
{ 
    private readonly IDialogService _dialogService;

    protected abstract TViewModel NewViewModel(TModel model);

    private TModel _currentModel;

    public SelectorViewModelBase(IServiceContainer container, IModelDataService<TId, TModel> dataService)
        : this(container, dataService, EnumUtil.GetValues<TId>().ToArray()) { }

    public SelectorViewModelBase(IServiceContainer container, IModelDataService<TId, TModel> dataService, TId[] items)
    {
        _dialogService = container.Resolve<IDialogService>();
        DataService = dataService;
        Items = items;

        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
    }

    private readonly IModelDataService<TId, TModel> DataService;

    private TViewModel _nestedViewModel;
    public TViewModel NestedViewModel
    {
        get => _nestedViewModel;
        set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
    }

    public TId[] Items { get; }

    private TId _selected;
    public TId Selected
    {
        get => _selected;
        set
        {
            Save();
            _selected = value;
            Refresh();
        }
    }

    /// <summary>
    /// Reload without saving.
    /// </summary>
    public void Refresh()
    {
        try
        {
            _currentModel = DataService.Retrieve(_selected);
            NestedViewModel = NewViewModel(_currentModel);
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Error,
                title: $"Error saving data in {GetType().Name}",
                message: e.Message
            ));
        }

    }

    public virtual void Save()
    {
        if (_currentModel != null && _selected != null)
        {
            try
            {
                DataService.Save(_selected, _currentModel);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    type: MessageBoxType.Error,
                    title: $"Error saving data in {GetType().Name}",
                    message: e.Message
                ));
            }
        }
    }

    public ICommand CopyCommand { get; }

    public ICommand PasteCommand { get; }


    private void Copy()
    {
        Clipboard.SetText(_currentModel.Serialize());
    }

    private void Paste()
    {
        string text = Clipboard.GetText();

        if (_currentModel.TryDeserialize(text))
        {
            // trigger visual update
            NestedViewModel = NewViewModel(_currentModel);
        }
        else
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Warning,
                title: "Invalid Paste Data",
                message: "The data that you pasted is invalid. Make sure you have the right label and length." +
                          $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{_currentModel.Serialize()}"
            ));
        }
    }
}
