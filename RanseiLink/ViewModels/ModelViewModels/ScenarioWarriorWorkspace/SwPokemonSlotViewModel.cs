using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class SwPokemonSlotViewModel : ViewModelBase
{
    private readonly int _slot;
    private bool _isSelected;
    private object _nestedVm;
    private readonly ScenarioPokemonViewModel _scenarioPokemonVm;
    private readonly ScenarioWarrior _warrior;
    private readonly ScenarioId _scenario;
    private readonly IChildScenarioPokemonService _spService;
    private ImageSource _pokemonImage;
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly SwMiniViewModel _parent;

    public SwPokemonSlotViewModel(SwMiniViewModel parent, ScenarioId scenario, ScenarioWarrior warrior, int slot, ScenarioPokemonViewModel scenarioPokemonVm, 
        IChildScenarioPokemonService spService, 
        ICachedSpriteProvider spriteProvider)
    {
        _parent = parent;
        _slot = slot;
        _scenario = scenario;
        _warrior = warrior;
        _scenarioPokemonVm = scenarioPokemonVm;
        _spService = spService;
        _spriteProvider = spriteProvider;

        UpdateNested();
        UpdatePokemonImage();

        _scenarioPokemonVm.PropertyChanged += ScenarioPokemonVm_PropertyChanged;
    }

    private void ScenarioPokemonVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ScenarioPokemonViewModel.Pokemon))
        {
            UpdatePokemonImage();
        }
    }

    public List<SelectorComboBoxItem> ScenarioPokemonItems => _parent.ScenarioPokemonItems;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (RaiseAndSetIfChanged(ref _isSelected, value) && value)
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
            if (RaiseAndSetIfChanged(ScenarioPokemonId, value, v => _warrior.SetScenarioPokemon(_slot, value)))
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

    public object NestedVm
    {
        get => _nestedVm;
        set => RaiseAndSetIfChanged(ref _nestedVm, value);
    }

    public ImageSource PokemonImage
    {
        get => _pokemonImage;
        set => RaiseAndSetIfChanged(ref _pokemonImage, value);
    }

    private void UpdatePokemonImage()
    {
        if (_warrior.ScenarioPokemonIsDefault(_slot))
        {
            PokemonImage = null;
            return;
        }
        int image = (int)_spService.Retrieve(ScenarioPokemonId).Pokemon;
        PokemonImage = _spriteProvider.GetSprite(SpriteType.StlPokemonS, image);
    }
}
