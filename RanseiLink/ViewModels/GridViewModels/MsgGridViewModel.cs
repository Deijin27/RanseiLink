using RanseiLink.Core.Services;
using RanseiLink.Services;
using RanseiLink.ViewModels.ModelViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class MsgGridViewModel : ViewModelBase, IGridViewModel<MsgViewModel>, ISaveableRefreshable
{
    private readonly ICachedMsgBlockService _cachedMsgBlockService;
    public MsgGridViewModel(IServiceContainer container, IEditorContext context)
    {
        _cachedMsgBlockService = context.CachedMsgBlockService;
        for (int i = 0; i < _cachedMsgBlockService.BlockCount; i++)
        {
            var block = _cachedMsgBlockService.Retrieve(i);
            for (int j = 0; j < block.Count; j++)
            {
                _allItems.Add(new MsgViewModel(i, j, block));
            }
        }
        Items = new ObservableCollection<MsgViewModel>(_allItems);

        SearchCommand = new RelayCommand(Search, () => !_searching);
    }

    public string SearchTerm { get; set; }

    public int FrozenColumnCount => 1;

    public ObservableCollection<MsgViewModel> Items { get; }

    private readonly List<MsgViewModel> _allItems = new();

    public void Refresh()
    {
        // no need to refresh when switching tabs because this is kept in memory
        // but probably do need to after running a plugin.
    }

    public void Save()
    {
        _cachedMsgBlockService.SaveChangedBlocks();
    }

    public ICommand SearchCommand { get; }
    private bool _searching;
    private void Search()
    {
        _searching = true;
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
            if (item.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                Items.Add(item);
            }
        }
        _searching = false;
    }
}
