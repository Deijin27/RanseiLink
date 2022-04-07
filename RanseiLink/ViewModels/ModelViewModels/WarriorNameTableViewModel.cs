using RanseiLink.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RanseiLink.ViewModels;

public interface IWarriorNameTableViewModel
{
    void SetModel(WarriorNameTable model);
}

public class WarriorNameTableItem : ViewModelBase
{
    private readonly WarriorNameTable _table;
    public WarriorNameTableItem(uint index, WarriorNameTable table)
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
public class WarriorNameTableViewModel : ViewModelBase, IWarriorNameTableViewModel
{
    public WarriorNameTableViewModel()
    {
    }

    public void SetModel(WarriorNameTable model)
    {
        var lst = new List<WarriorNameTableItem>();
        for (uint i = 0; i < WarriorNameTable.EntryCount; i++)
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
            if (RaiseAndSetIfChanged(ref _searchTerm, value))
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
        }
        Items.Clear();
        
        foreach (var item in _allItems)
        {
            if (item.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            {
                Items.Add(item);
            }
        }
    }
}
