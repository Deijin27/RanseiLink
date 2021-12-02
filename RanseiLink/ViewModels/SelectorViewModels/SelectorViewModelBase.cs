using RanseiLink.Core;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase, ISaveableRefreshable
    where TViewModel : IViewModelForModel<TModel>, new()
    where TModel : IDataWrapper
{
    private readonly IDialogService _dialogService;
    public SelectorViewModelBase(IDialogService dialogService, TId initialSelected, IModelDataService<TId, TModel> dataService)
        : this(dialogService, initialSelected, dataService, EnumUtil.GetValues<TId>().ToArray()) { }

    public SelectorViewModelBase(IDialogService dialogService, TId initialSelected, IModelDataService<TId, TModel> dataService, TId[] items)
    {
        _dialogService = dialogService;
        DataService = dataService;
        Selected = initialSelected;
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
            TModel model = DataService.Retrieve(_selected);
            NestedViewModel = new TViewModel() { Model = model };
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                icon: MessageBoxIcon.Error,
                title: $"Error saving data in {GetType().Name}",
                message: e.Message
            ));
        }

    }

    public virtual void Save()
    {
        if (NestedViewModel != null && _selected != null)
        {
            try
            {
                DataService.Save(_selected, NestedViewModel.Model);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    icon: MessageBoxIcon.Error,
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
        Clipboard.SetText(NestedViewModel.Model.Serialize());
    }

    private void Paste()
    {
        string text = Clipboard.GetText();

        if (NestedViewModel.Model.TryDeserialize(text))
        {
            NestedViewModel = new TViewModel { Model = NestedViewModel.Model };
        }
        else
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                icon: MessageBoxIcon.Warning,
                title: "Invalid Paste Data",
                message: "The data that you pasted is invalid. Make sure you have the right label and length." +
                          $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{NestedViewModel.Model.Serialize()}"
            ));
        }
    }
}
