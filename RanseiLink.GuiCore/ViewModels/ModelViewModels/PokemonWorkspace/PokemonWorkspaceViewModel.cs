#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RanseiLink.GuiCore.ViewModels;

public class PokemonWorkspaceViewModel : ViewModelBase
{
    private PokemonMiniViewModel? _selectedMiniVm;
    private readonly IPokemonService _pokemonService;
    private readonly List<PokemonMiniViewModel> _allMiniVms = [];

    public PokemonWorkspaceViewModel(ICachedSpriteProvider cachedSpriteProvider, PokemonViewModel pokemonViewModel, IPokemonService pokemonService)
    {
        PokemonViewModel = pokemonViewModel;
        _pokemonService = pokemonService;
        var selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) SelectById(pk.Id); });

        foreach (var pid in pokemonService.ValidIds())
        {
            var pokemon = pokemonService.Retrieve(pid);
            var miniVm = new PokemonMiniViewModel(cachedSpriteProvider, pokemon, pid, selectPokemonCommand);
            _allMiniVms.Add(miniVm);
        }
        Items = new(_allMiniVms);
        Items.CollectionChanged += Items_CollectionChanged;

        SelectById(0);

        PokemonViewModel.PropertyChanged += PokemonViewModel_PropertyChanged;
    }

    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
    }

    private bool _ignorePropertyChanged;

    private void PokemonViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_ignorePropertyChanged)
        {
            return;
        }
        // keep the mini views up to date with the data as the user is changing it
        var name = e.PropertyName;
        if (name == nameof(PokemonViewModel.SmallSpritePath))
        {
            name = nameof(PokemonMiniViewModel.Image);
        }
        SelectedMiniVm?.NotifyPropertyChanged(name);
    }

    public void SelectById(int pokemonId)
    {
        if (!(_pokemonService.ValidateId(pokemonId)))
        {
            return;
        }
        _ignorePropertyChanged = true;
        var model = _pokemonService.Retrieve(pokemonId);
        SelectedMiniVm = _allMiniVms[pokemonId];
        PokemonViewModel.SetModel((PokemonId)pokemonId, model);
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

    public PokemonMiniViewModel? SelectedMiniVm
    {
        get => _selectedMiniVm;
        set => RaiseAndSetIfChanged(ref _selectedMiniVm, value);
    }

    public PokemonViewModel PokemonViewModel { get; }

    public ObservableCollection<PokemonMiniViewModel> Items { get; }
}
