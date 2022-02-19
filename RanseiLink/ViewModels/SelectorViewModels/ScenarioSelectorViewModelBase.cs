using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public abstract class ScenarioSelectorViewModelBase<TModel, TViewModel> : ViewModelBase, ISaveableRefreshable
    where TModel : IDataWrapper
{
    private readonly IDialogService _dialogService;

    protected abstract TViewModel NewViewModel(ScenarioId scenarioId, uint id, TModel model);

    private TModel _currentModel;

    protected ScenarioSelectorViewModelBase(IServiceContainer container, uint minIndex, uint maxIndex)
    {
        _dialogService = container.Resolve<IDialogService>();
        MinIndex = minIndex;
        MaxIndex = maxIndex;
        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
    }

    protected void Init()
    {
        _selectedScenario = ScenarioId.TheLegendOfRansei;
        _selectedItem = 0;
        try
        {
            _currentModel = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario, SelectedItem, _currentModel);
            NestedViewModel = vm;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Error,
                title: $"Error retrieving initial data in {GetType().Name}",
                message: e.ToString()
            ));
        }
    }


    private TViewModel _nestedViewModel;
    public TViewModel NestedViewModel
    {
        get => _nestedViewModel;
        set
        {
            if (!value.Equals(_nestedViewModel))
            {
                if (_nestedViewModel is ISaveable saveable)
                {
                    try
                    {
                        saveable.Save();
                    }
                    catch (Exception e)
                    {
                        _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                            type: MessageBoxType.Error,
                            title: $"Error saving nested saveable {saveable.GetType().Name} in {GetType().Name}",
                            message: e.ToString()
                        ));
                    }

                }
                _nestedViewModel = value;
                RaisePropertyChanged();
            }
        }
    }

    // min and max indexes bound to by number box
    public uint MinIndex { get; }
    public uint MaxIndex { get; }

    private uint _selectedItem;
    public uint SelectedItem
    {
        get => _selectedItem;
        set
        {
            Save();
            try
            {
                if (value != _selectedItem)
                {
                    _currentModel = RetrieveModel(SelectedScenario, value);
                    var vm = NewViewModel(SelectedScenario, value, _currentModel);
                    NestedViewModel = vm;
                    _selectedItem = value;
                    RaisePropertyChanged();
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    type: MessageBoxType.Error,
                    title: $"Error retrieving new selection data in {GetType().Name}",
                    message: e.ToString()
            ));
            }
        }
    }

    public ScenarioId[] ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToArray();

    private ScenarioId _selectedScenario;
    public ScenarioId SelectedScenario
    {
        get => _selectedScenario;
        set
        {
            Save();
            try
            {
                if (_selectedScenario != value)
                {
                    _currentModel = RetrieveModel(value, SelectedItem);
                    var vm = NewViewModel(value, SelectedItem, _currentModel);
                    NestedViewModel = vm;
                    _selectedScenario = value;
                    RaisePropertyChanged();
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    type: MessageBoxType.Error,
                    title: $"Error retrieving new selection data in {GetType().Name}",
                    message: e.ToString()
                    ));
            }
        }
    }

    protected abstract TModel RetrieveModel(ScenarioId scenario, uint index);
    protected abstract void SaveModel(ScenarioId scenario, uint index, TModel model);

    public virtual void Save()
    {
        if (NestedViewModel != null)
        {
            try
            {
                if (_nestedViewModel is ISaveable saveable)
                {
                    saveable.Save();
                }
                SaveModel(SelectedScenario, SelectedItem, _currentModel);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    type: MessageBoxType.Error,
                    title: $"Error saving data in {GetType().Name}",
                    message: e.ToString()
                    ));
            }
        }
    }

    public void Refresh()
    {
        try
        {
            _currentModel = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario, SelectedItem, _currentModel);
            NestedViewModel = vm;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                 type: MessageBoxType.Error,
                 title: $"Error retrieving data in {GetType().Name}",
                 message: e.ToString()
             ));
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
            var newvm = NewViewModel(SelectedScenario, SelectedItem, _currentModel);
            _nestedViewModel = newvm;
            RaisePropertyChanged(nameof(NestedViewModel));
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
