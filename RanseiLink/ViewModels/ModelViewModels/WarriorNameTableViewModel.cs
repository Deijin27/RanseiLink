using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class WarriorNameTableItem : ViewModelBase
{
    private readonly IWarriorNameTable _table;
    public WarriorNameTableItem(uint index, IWarriorNameTable table)
    {
        Index = index;
        _table = table;
    }
    public uint Index { get; }

    public string Name
    {
        get => _table.GetEntry(Index);
        set => RaiseAndSetIfChanged(Name, value, v => _table.SetEntry(Index, v));
    }
}
public class WarriorNameTableViewModel : ViewModelBase, ISaveableRefreshable
{
    private readonly IDialogService _dialogService;
    private readonly IBaseWarriorService _service;
    private IWarriorNameTable _model;

    public WarriorNameTableViewModel(IDialogService dialogService, IBaseWarriorService dataService)
    {
        _dialogService = dialogService;
        _service = dataService;
        Refresh();

        PasteCommand = new RelayCommand(Paste);
        CopyCommand = new RelayCommand(Copy);
    }

    public IReadOnlyList<WarriorNameTableItem> Items { get; private set; }

    public void Save()
    {
        try
        {
            _service.SaveNameTable(_model);
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

    public void Refresh()
    {
        try
        {
            _model = _service.RetrieveNameTable();
            var lst = new List<WarriorNameTableItem>();
            for (uint i = 0; i < WarriorNameTable.EntryCount; i++)
            {
                lst.Add(new WarriorNameTableItem(i, _model));
            }
            Items = lst;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Error,
                title: $"Error retrieving data in {GetType().Name}",
                message: e.Message
            ));
        }
    }

    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }

    private void Copy()
    {
        Clipboard.SetText(_model.Serialize());
    }

    private void Paste()
    {
        string text = Clipboard.GetText();

        if (_model.TryDeserialize(text))
        {
            var lst = new List<WarriorNameTableItem>();
            for (uint i = 0; i < WarriorNameTable.EntryCount; i++)
            {
                lst.Add(new WarriorNameTableItem(i, _model));
            }
            Items = lst;
        }
        else
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                type: MessageBoxType.Warning,
                title: "Invalid Paste Data",
                message: "The data that you pasted is invalid. Make sure you have the right label." +
                          $"\n\nYou pasted:\n\n{text}\n\nWhat was expected was something like:\n\n{_model.Serialize()}"
            ));
        }
    }
}
