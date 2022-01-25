using RanseiLink.Core;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase, ISaveableRefreshable
{ 
    private readonly IDialogService _dialogService;

    protected abstract TViewModel NewViewModel(TModel model);

    protected TModel _currentModel;

    public SelectorViewModelBase(IServiceContainer container, IModelDataService<TId, TModel> dataService)
        : this(container, dataService, EnumUtil.GetValues<TId>().ToArray()) { }

    public SelectorViewModelBase(IServiceContainer container, IModelDataService<TId, TModel> dataService, TId[] items)
    {
        _dialogService = container.Resolve<IDialogService>();
        DataService = dataService;
        Items = items;

        if (typeof(IDataWrapper).IsAssignableFrom(typeof(TModel)))
        {
            CopyPasteVisible = true;
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            ImportFileCommand = new RelayCommand(ImportFile);
            ExportFileCommand = new RelayCommand(ExportFile);
        }
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
            if (_selected?.Equals(value) != true)
            {
                Save();
                _selected = value;
                Refresh();
                RaisePropertyChanged();
            }
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

    private bool _copyPasteVisible = false;
    public bool CopyPasteVisible
    {
        get => _copyPasteVisible;
        set => RaiseAndSetIfChanged(ref _copyPasteVisible, value);
    }


    private void Copy()
    {
        if (_currentModel is IDataWrapper dw)
        {
            Clipboard.SetText(dw.Serialize());
        }
    }

    private void Paste()
    {
        if (_currentModel is IDataWrapper dw)
        {
            string text = Clipboard.GetText();

            if (dw.TryDeserialize(text))
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
                              $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{dw.Serialize()}"
                ));
            }
        }
    }

    public bool SupportsImportExportFile { get; protected set; }
    public ICommand ImportFileCommand { get; }
    public ICommand ExportFileCommand { get; }
    protected virtual void ExportFile()
    {

    }

    protected virtual void ImportFile()
    {

    }
}
