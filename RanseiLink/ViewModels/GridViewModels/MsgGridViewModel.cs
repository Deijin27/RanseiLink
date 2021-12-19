using RanseiLink.Core.Services;
using RanseiLink.Services;
using RanseiLink.ViewModels.ModelViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

// This doesn't actually need to be saveable refreshable but whatever
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

        SearchCommand = new RelayCommand(Search, () => !_busy);
        ReplaceAllCommand = new RelayCommand(ReplaceAll, () => !_busy);
        ClearCommand = new RelayCommand(() => SearchTerm = "", () => !_busy);
    }

    private bool _matchCase = false;
    public bool MatchCase
    {
        get => _matchCase;
        set
        {
            if (RaiseAndSetIfChanged(ref _matchCase, value))
            {
                Search();
            }
        }
    }

    private bool _useRegex = false;
    public bool UseRegex
    {
        get => _useRegex;
        set
        {
            if (RaiseAndSetIfChanged(ref _useRegex, value))
            {
                RaisePropertyChanged(nameof(RegexInvalid));
                Search();
            }
        }
    }


    private bool _replaceVisible = false;
    public bool ReplaceVisible
    {
        get => _replaceVisible;
        set
        {
            if (RaiseAndSetIfChanged(ref _replaceVisible, value))
            {
                Search();
            }
        }
    }

    private string _searchTerm = "";
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (RaiseAndSetIfChanged(ref _searchTerm, value))
            {
                RaisePropertyChanged(nameof(RegexInvalid));
                Search();
            }
        }
    }

    private string _replaceWith = "";
    public string ReplaceWith
    {
        get => _replaceWith;
        set => RaiseAndSetIfChanged(ref _replaceWith, value);
    }

    public bool RegexInvalid => UseRegex && !TryGenerateRegex(SearchTerm, RegexOptions.CultureInvariant, out var _);

    public int FrozenColumnCount => 1;

    public ObservableCollection<MsgViewModel> Items { get; }

    private readonly List<MsgViewModel> _allItems = new();

    public void Refresh()
    {
        // refresh isn't necessary because this is cached.
        // The cache is cleared and refreshed properly where necessary in MainEditorViewModel
    }

    public void Save()
    {
        // save is done outside because the cache is used in multiple places and it's quick anyway
    }

    public ICommand SearchCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ReplaceAllCommand { get; }

    private bool _busy;
    private void Search()
    {
        _busy = true;
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
        if (UseRegex)
        {
            var options = RegexOptions.CultureInvariant;
            if (!MatchCase)
            {
                options |= RegexOptions.IgnoreCase;
            }
            
            if (!TryGenerateRegex(searchTerm, options, out var rx))
            {
                return;
            }
            
            foreach (var item in _allItems)
            {
                if (rx.IsMatch(item.Text))
                {
                    Items.Add(item);
                }
            }
        }
        else
        {
            StringComparison comparison = MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            foreach (var item in _allItems)
            {
                if (item.Text.Contains(searchTerm, comparison))
                {
                    Items.Add(item);
                }
            }
        }
        
        _busy = false;
    }

    private static bool TryGenerateRegex(string pattern, RegexOptions options, out Regex regex)
    {
        try
        {
            regex = new Regex(pattern, options);
            return true;
        }
        catch (ArgumentException)
        {
            regex = null;
            return false;
        }
    }

    public void ReplaceAll()
    {
        _busy = true;

        string replaceTerm = ReplaceWith;
        string searchTerm = SearchTerm;

        Items.Clear();

        if (UseRegex)
        {
            var options = RegexOptions.CultureInvariant;
            if (!MatchCase)
            {
                options |= RegexOptions.IgnoreCase;
            }
            if (!TryGenerateRegex(searchTerm, options, out var rx))
            {
                return;
            }
            foreach (var item in _allItems)
            {
                if (rx.IsMatch(item.Text))
                {
                    rx.Replace(item.Text, replaceTerm);
                    Items.Add(item);
                }
            }
        }
        else
        {
            StringComparison comparison = MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            foreach (var item in _allItems)
            {
                if (item.Text.Contains(searchTerm, comparison))
                {
                    item.Text = item.Text.Replace(searchTerm, replaceTerm, comparison);
                    Items.Add(item);
                }
            }
        }
    }

}
