#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class AbilityMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IPokemonService _pokemonService;
    private readonly Ability _model;
    private readonly int _id;

    public AbilityMiniViewModel(ICachedSpriteProvider spriteProvider, IPokemonService pokemonService, Ability model, int id, ICommand selectCommand)
    {
        _spriteProvider = spriteProvider;
        _pokemonService = pokemonService;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, v => _model.Name = v);
    }

    public object? Image
    {
        get
        {
            var idEnum = (AbilityId)_id;

            foreach (var pokemonId in _pokemonService.ValidIds())
            {
                var pokemon = _pokemonService.Retrieve(pokemonId);
                if (pokemon.HasAbility(idEnum))
                {
                    return _spriteProvider.GetSprite(SpriteType.StlPokemonS, pokemonId);
                }
            }
            return null;
        }
    }

    public ICommand SelectCommand { get; }

    public bool MatchSearchTerm(string searchTerm)
    {
        if (Name.ContainsIgnoreCaseAndAccents(searchTerm))
        {
            return true;
        }


        return false;
    }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(Name):
                RaisePropertyChanged(name);
                break;
        }
    }
}
