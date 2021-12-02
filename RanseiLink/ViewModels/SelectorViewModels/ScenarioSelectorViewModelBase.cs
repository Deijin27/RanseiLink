using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public abstract class ScenarioSelectorViewModelBase<TModel, TViewModel> : ViewModelBase, ISaveableRefreshable
    where TViewModel : IViewModelForModel<TModel>
    where TModel : IDataWrapper
{
    private readonly IDialogService _dialogService;
    public ScenarioSelectorViewModelBase(IDialogService dialogService, Func<ScenarioId, TViewModel> newViewModel, uint minIndex, uint maxIndex)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        NewViewModel = newViewModel ?? throw new ArgumentNullException(nameof(newViewModel));
        MinIndex = minIndex;
        MaxIndex = maxIndex;
        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
    }

    private readonly Func<ScenarioId, TViewModel> NewViewModel;

    protected void Init()
    {
        _selectedScenario = ScenarioId.TheLegendOfRansei;
        _selectedItem = 0;
        try
        {
            TModel model = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario);
            vm.Model = model;
            NestedViewModel = vm;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                icon: MessageBoxIcon.Error,
                title: $"Error retrieving initial data in {GetType().Name}",
                message: e.Message
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
                            icon: MessageBoxIcon.Error,
                            title: $"Error saving nested saveable {saveable.GetType().Name} in {GetType().Name}",
                            message: e.Message
                        ));
                    }

                }
                _nestedViewModel = value;
                RaisePropertyChanged();
            }
        }
    }

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
                TModel model = RetrieveModel(SelectedScenario, value);
                var vm = NewViewModel(SelectedScenario);
                vm.Model = model;
                NestedViewModel = vm;
                _selectedItem = value;
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    icon: MessageBoxIcon.Error,
                    title: $"Error retrieving new selection data in {GetType().Name}",
                    message: e.Message
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
                TModel model = RetrieveModel(value, SelectedItem);
                var vm = NewViewModel(value);
                vm.Model = model;
                NestedViewModel = vm;
                _selectedScenario = value;
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    icon: MessageBoxIcon.Error,
                    title: $"Error retrieving new selection data in {GetType().Name}",
                    message: e.Message
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
                SaveModel(SelectedScenario, SelectedItem, NestedViewModel.Model);
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

    public void Refresh()
    {
        try
        {
            TModel model = RetrieveModel(SelectedScenario, SelectedItem);
            var vm = NewViewModel(SelectedScenario);
            vm.Model = model;
            NestedViewModel = vm;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                 icon: MessageBoxIcon.Error,
                 title: $"Error retrieving data in {GetType().Name}",
                 message: e.Message
             ));
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
            var newvm = NewViewModel(SelectedScenario);
            newvm.Model = NestedViewModel.Model;
            _nestedViewModel = newvm;
            RaisePropertyChanged(nameof(NestedViewModel));
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
