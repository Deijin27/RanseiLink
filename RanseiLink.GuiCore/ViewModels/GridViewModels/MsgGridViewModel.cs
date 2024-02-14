using RanseiLink.Core.Services;
using RanseiLink.Core.Text;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RanseiLink.GuiCore.ViewModels;

public class MsgGridViewModel : ViewModelBase, IGridViewModel<MsgViewModel>
{
    private readonly ICachedMsgBlockService _cachedMsgBlockService;

    public MsgGridViewModel(ICachedMsgBlockService cachedMsgBlockService)
    {
        _cachedMsgBlockService = cachedMsgBlockService;
        
        Items = new ObservableCollection<MsgViewModel>();

        SearchCommand = new RelayCommand(Search, () => !Busy);
        ReplaceAllCommand = new RelayCommand(ReplaceAll, () => !Busy);
        ClearCommand = new RelayCommand(() => SearchTerm = "", () => !Busy);
        AddCommand = new RelayCommand(Add, CanAdd);
        RemoveCommand = new RelayCommand(Remove, CanRemove);

        Reload();

        _cachedMsgBlockService.MessageAdded += CachedMsgBlockService_MessageAdded;
        _cachedMsgBlockService.MessageRemoved += CachedMsgBlockService_MessageRemoved;


    }

    public void Reload()
    {
        _allItems.Clear();
        for (int i = 0; i < _cachedMsgBlockService.BlockCount; i++)
        {
            var block = _cachedMsgBlockService.Retrieve(i);
            for (int j = 0; j < block.Count; j++)
            {
                _allItems.Add(new MsgViewModel(i, j, block));
            }
        }
        Search();
    }

    private void CachedMsgBlockService_MessageRemoved(object? sender, MessageRemovedArgs e)
    {
        var vmIndex = _allItems.FindIndex(x => x.Message == e.Message);
        if (vmIndex != -1)
        {
            return;
        }
        var vm = _allItems[vmIndex];
        _allItems.RemoveAt(vmIndex);
        // Fixup index in block values
        for (int i = vmIndex; i < _allItems.Count; i++)
        {
            var item = _allItems[i];
            if (item.BlockId != e.BlockId)
            {
                break;
            }
            item.Id--;
        }
        Search();
    }

    private void CachedMsgBlockService_MessageAdded(object? sender, MessageAddedArgs e)
    {
        var vm = new MsgViewModel(e.BlockId, e.IndexInBlock, _cachedMsgBlockService.Retrieve(e.BlockId));
        var indexToInsert = _allItems.FindIndex(x => x.BlockId == e.BlockId && x.Id == e.IndexInBlock);
        _allItems.Insert(indexToInsert, vm);
        // Fixup index in block values
        for (int i = indexToInsert + 1; i < _allItems.Count; i++)
        {
            var item = _allItems[i];
            if (item.BlockId != e.BlockId)
            {
                break;
            }
            item.Id++;
        }
        
        Search();
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

    private bool _addRemoveVisible = false;
    public bool AddRemoveVisible
    {
        get => _addRemoveVisible;
        set => RaiseAndSetIfChanged(ref _addRemoveVisible, value);
    }

    private bool _replaceVisible = false;
    public bool ReplaceVisible
    {
        get => _replaceVisible;
        set => RaiseAndSetIfChanged(ref _replaceVisible, value);
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

    private MsgViewModel? _selectedItem;
    public MsgViewModel? SelectedItem
    {
        get => _selectedItem;
        set => RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand RemoveCommand { get; }
    private bool CanAdd()
    {
        if (Busy)
        {
            return false;
        }
        if (SelectedItem == null)
        {
            return false;
        }
        if (!SelectedItem.Block.CanAdd(SelectedItem.GroupId))
        {
            return false;
        }
        return true;
    }

    private bool CanRemove()
    {
        if (Busy)
        {
            return false;
        }
        if (SelectedItem == null)
        {
            return false;
        }
        if (!SelectedItem.Block.CanRemove(SelectedItem.GroupId))
        {
            return false;
        }
        return true;
    }

    private void Remove()
    {
        SelectedItem?.Block.Remove(SelectedItem.Message);
        RemoveCommand.RaiseCanExecuteChanged();
        AddCommand.RaiseCanExecuteChanged();
    }

    private void Add()
    {
        SelectedItem?.Block.Add(SelectedItem.GroupId);
        RemoveCommand.RaiseCanExecuteChanged();
        AddCommand.RaiseCanExecuteChanged();
    }

    public RelayCommand SearchCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand ReplaceAllCommand { get; }

    private bool _busy;

    private bool Busy
    {
        get => _busy;
        set
        {
            if (_busy != value)
            {
                _busy = value;
                SearchCommand.RaiseCanExecuteChanged();
                ClearCommand.RaiseCanExecuteChanged();
                ReplaceAllCommand.RaiseCanExecuteChanged();
                AddCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private void Search()
    {
        if (Busy)
        {
            return;
        }

        Busy = true;
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
                Busy = false;
                return;
            }
            
            for (var i = 0; i < _allItems.Count; i++)
            {
                var item = _allItems[i];
                if (rx.IsMatch(item.Text) || rx.IsMatch(item.BoxConfig) || rx.IsMatch(item.Context))
                {
                    Items.Add(item);
                    if (item.Context.Contains($"{{{PnaConstNames.MultiStart}:", StringComparison.InvariantCulture))
                    {
                        i++;
                        Items.Add(_allItems[i]);
                        i++;
                        Items.Add(_allItems[i]);
                    }
                }
            }
        }
        else
        {
            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            // IgnoreNonSpace ignores accents
            var options = CompareOptions.IgnoreNonSpace;
            if (!MatchCase)
            {
                options |= CompareOptions.IgnoreCase;
            }
            for (var i = 0; i < _allItems.Count; i++)
            {
                var item = _allItems[i];
                if (compareInfo.IndexOf(item.Text, searchTerm, options) != -1 
                 || compareInfo.IndexOf(item.Context, searchTerm, options) != -1
                 || compareInfo.IndexOf(item.BoxConfig, searchTerm, options) != -1
                 )
                {
                    Items.Add(item);
                    if (item.Context.Contains($"{{{PnaConstNames.MultiStart}:", StringComparison.InvariantCulture))
                    {
                        i++;
                        Items.Add(_allItems[i]);
                        i++;
                        Items.Add(_allItems[i]);
                    }
                }
            }
        }
        
        Busy = false;
    }

    private static bool TryGenerateRegex(string pattern, RegexOptions options, [NotNullWhen(true)] out Regex? regex)
    {
        options |= RegexOptions.Compiled;
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
        Busy = true;

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
                Busy = false;
                return;
            }
            foreach (var item in _allItems)
            {
                if (rx.IsMatch(item.Text) || rx.IsMatch(item.BoxConfig) || rx.IsMatch(item.Context))
                {
                    rx.Replace(item.Text, replaceTerm);
                    rx.Replace(item.BoxConfig, replaceTerm);
                    rx.Replace(item.Context, replaceTerm);
                    Items.Add(item);
                }
            }
        }
        else
        {
            StringComparison comparison = MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            foreach (var item in _allItems)
            {
                if (item.Text.Contains(searchTerm, comparison) || item.Context.Contains(searchTerm, comparison) || item.BoxConfig.Contains(searchTerm, comparison))
                {
                    item.Text = item.Text.Replace(searchTerm, replaceTerm, comparison);
                    item.BoxConfig = item.BoxConfig.Replace(searchTerm, replaceTerm, comparison);
                    item.Context = item.Context.Replace(searchTerm, replaceTerm, comparison);
                    Items.Add(item);
                }
            }
        }
        Busy = false;
    }

    public void UnhookEvents()
    {
        _cachedMsgBlockService.MessageAdded -= CachedMsgBlockService_MessageAdded;
        _cachedMsgBlockService.MessageRemoved -= CachedMsgBlockService_MessageRemoved;
    }
}
