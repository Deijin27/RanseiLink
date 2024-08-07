﻿#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;

public class SwPokemonSlotViewModel : ViewModelBase
{
    private readonly int _slot;
    private bool _isSelected;
    private object? _nestedVm;
    private readonly ScenarioPokemonViewModel _scenarioPokemonVm;
    private readonly ScenarioWarrior _warrior;
    private readonly ScenarioId _scenario;
    private readonly IChildScenarioPokemonService _spService;
    private readonly ICachedSpriteProvider _spriteProvider;

    public SwPokemonSlotViewModel(
        List<SelectorComboBoxItem> scenarioPokemonItems, 
        ScenarioId scenario, 
        ScenarioWarrior warrior, 
        int slot, 
        ScenarioPokemonViewModel scenarioPokemonVm, 
        IChildScenarioPokemonService spService, 
        ICachedSpriteProvider spriteProvider)
    {
        ScenarioPokemonItems = scenarioPokemonItems;
        _slot = slot;
        _scenario = scenario;
        _warrior = warrior;
        _scenarioPokemonVm = scenarioPokemonVm;
        _spService = spService;
        _spriteProvider = spriteProvider;

        UpdateNested();
        UpdatePokemonImage();
    }

    public List<SelectorComboBoxItem> ScenarioPokemonItems { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value) && value)
            {
                UpdateNested();
            }
        }
    }

    public int ScenarioPokemonId
    {
        get => _warrior.GetScenarioPokemon(_slot);
        set
        {
            if (SetProperty(ScenarioPokemonId, value, v => _warrior.SetScenarioPokemon(_slot, value)))
            {
                UpdateNested();
                UpdatePokemonImage();
            }
        }
    }

    public void UpdateNested()
    {
        if (_warrior.ScenarioPokemonIsDefault(_slot))
        {
            NestedVm = null;
        }
        else
        {
            _scenarioPokemonVm.SetModel(_scenario, ScenarioPokemonId, _spService.Retrieve(ScenarioPokemonId));
            NestedVm = _scenarioPokemonVm;
        }
    }

    public object? NestedVm
    {
        get => _nestedVm;
        set => SetProperty(ref _nestedVm, value);
    }

    public object? PokemonImage => _spriteProvider.GetSprite(SpriteType.StlPokemonS, _pokemonImageId);
    private int _pokemonImageId;
    public void UpdatePokemonImage()
    {
        if (_warrior.ScenarioPokemonIsDefault(_slot))
        {
            _pokemonImageId = -1;
        }
        else
        {
            _pokemonImageId = (int)_spService.Retrieve(ScenarioPokemonId).Pokemon;
        }
        RaisePropertyChanged(nameof(PokemonImage));
    }
}
