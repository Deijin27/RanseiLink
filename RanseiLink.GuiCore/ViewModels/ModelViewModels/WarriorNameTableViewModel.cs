﻿#nullable enable
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;
using System.Collections.ObjectModel;
using System.Globalization;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorNameTableItem : ViewModelBase
{
    private readonly WarriorNameTable _table;
    public WarriorNameTableItem(int index, WarriorNameTable table)
    {
        Index = index;
        _table = table;
    }
    public int Index { get; }

    public string Name
    {
        get => _table.GetEntry(Index);
        set => SetProperty(Name, value, v => _table.SetEntry(Index, v));
    }
}
public class WarriorNameTableViewModel : ViewModelBase
{
    public WarriorNameTableViewModel()
    {
    }

    public void SetModel(WarriorNameTable model)
    {
        var lst = new List<WarriorNameTableItem>();
        for (int i = 0; i < WarriorNameTable.EntryCount; i++)
        {
            lst.Add(new WarriorNameTableItem(i, model));
        }
        _allItems.Clear();
        _allItems.AddRange(lst);
        Search();
    }

    public ObservableCollection<WarriorNameTableItem> Items { get; } = new();

    private readonly List<WarriorNameTableItem> _allItems = new();

    private string _searchTerm = "";
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                Search();
            }
        }
    }
    private void Search()
    {
        string searchTerm = SearchTerm;
        if (string.IsNullOrEmpty(searchTerm))
        {
            Items.Clear();
            foreach (var item in _allItems)
            {
                Items.Add(item);
            }
            return;
        }
        Items.Clear();
        foreach (var item in _allItems)
        {
            if (item.Name.ContainsIgnoreCaseAndAccents(searchTerm))
            {
                Items.Add(item);
            }
        }
    }
}
