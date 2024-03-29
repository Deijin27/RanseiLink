using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorWorkspaceViewModel : ViewModelBase
{
    private WarriorMiniViewModel? _selectedMiniVm;
    private readonly IBaseWarriorService _warriorService;
    private readonly List<WarriorMiniViewModel> _allMiniVms = [];

    public WarriorWorkspaceViewModel(ICachedSpriteProvider cachedSpriteProvider, BaseWarriorViewModel warriorViewModel, IBaseWarriorService warriorService)
    {
        WarriorViewModel = warriorViewModel;
        _warriorService = warriorService;
        var selectWarriorCommand = new RelayCommand<WarriorMiniViewModel>(wa => { if (wa != null) SelectById(wa.Id); });

        foreach (var pid in warriorService.ValidIds())
        {
            var warrior = warriorService.Retrieve(pid);
            var miniVm = new WarriorMiniViewModel(cachedSpriteProvider, warriorService, warrior, pid, selectWarriorCommand);
            _allMiniVms.Add(miniVm);
        }
        Items = new(_allMiniVms);

        SelectById(0);

        WarriorViewModel.PropertyChanged += WarriorViewModel_PropertyChanged;
    }

    private bool _ignorePropertyChanged;

    private void WarriorViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_ignorePropertyChanged)
        {
            return;
        }
        // keep the mini views up to date with the data as the user is changing it
        var name = e.PropertyName;
        if (name == nameof(WarriorViewModel.SmallSpritePath))
        {
            name = nameof(WarriorMiniViewModel.Image);
        }
        SelectedMiniVm?.NotifyPropertyChanged(name);
    }

    public void SelectById(int WarriorId)
    {
        if (!(_warriorService.ValidateId(WarriorId)))
        {
            return;
        }
        _ignorePropertyChanged = true;
        var model = _warriorService.Retrieve(WarriorId);
        SelectedMiniVm = _allMiniVms[WarriorId];
        WarriorViewModel.SetModel((WarriorId)WarriorId, model);
        _ignorePropertyChanged = false;
    }

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (RaiseAndSetIfChanged(ref _searchText, value))
            {
                Search();
            }
        }
    }

    private void Search()
    {
        Items.Clear();

        if (string.IsNullOrWhiteSpace(_searchText))
        {
            foreach (var item in _allMiniVms)
            {
                Items.Add(item);
            }
            return;
        }

        var searchTerms = _searchText.Split();

        foreach (var item in _allMiniVms)
        {
            bool match = false;
            foreach (var term in searchTerms)
            {
                match = false;
                if (item.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    match = true;
                }

                if (Enum.TryParse<TypeId>(term, ignoreCase: true, out var type))
                {
                    if (item.Type1 == type || item.Type2 == type)
                    {
                        match = true;
                    }
                }

                // must match all terms
                if (!match)
                {
                    break;
                }
            }

            if (match)
            {
                Items.Add(item);
            }
        }

        if (Items.Count == 1)
        {
            SelectById(Items[0].Id);
        }
    }

    public WarriorMiniViewModel? SelectedMiniVm
    {
        get => _selectedMiniVm;
        set => RaiseAndSetIfChanged(ref _selectedMiniVm, value);
    }

    public BaseWarriorViewModel WarriorViewModel { get; }

    public ObservableCollection<WarriorMiniViewModel> Items { get; }
}
