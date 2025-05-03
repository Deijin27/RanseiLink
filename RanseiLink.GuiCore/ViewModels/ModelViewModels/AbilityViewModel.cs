using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class AbilityViewModel : ViewModelBase, IBigViewModel
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IPokemonService _pokemonService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public AbilityViewModel(
        ICachedMsgBlockService msgService,
        IJumpService jumpService,
        IPokemonService pokemonService,
        ICachedSpriteProvider cachedSpriteProvider)
    {
        _pokemonService = pokemonService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _msgService = msgService;

        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) jumpService.JumpTo(PokemonWorkspaceModule.Id, pk.Id); });
    }

    public PropertyCollectionViewModel Properties { get; } = new PropertyCollectionViewModel();

    public void SetModel(AbilityId id, Ability model)
    {
        _id = id;
        _model = model;
        //RaiseAllPropertiesChanged();

        Properties.Properties.Clear();
        Properties.Properties.Add(new StringPropertyViewModel("Name", () => _model.Name, v =>  _model.Name = v, _model.Name_MaxLength));
        Properties.Properties.Add(new ComboPropertyViewModel("Effect 1", () => (int)_model.Effect1, v => _model.Effect1 = (AbilityEffectId)v, SelectorComboBoxItemExtensions.GetForEnum<AbilityEffectId>()));
        Properties.Properties.Add(new IntPropertyViewModel("Amount", () => _model.Effect1Amount, v => _model.Effect1Amount = v, 0, Effect1Amount_Max));
        Properties.Properties.Add(new ComboPropertyViewModel("Effect 2", () => (int)_model.Effect2, v => _model.Effect2 = (AbilityEffectId)v, SelectorComboBoxItemExtensions.GetForEnum<AbilityEffectId>()));
        Properties.Properties.Add(new IntPropertyViewModel("Amount", () => _model.Effect2Amount, v => _model.Effect2Amount = v, 0, Effect2Amount_Max));
    }

    public void SetModel(int id, object model)
    {
        SetModel((AbilityId)id, (Ability)model);
    }

    private readonly ICommand _selectPokemonCommand;

    public List<PokemonMiniViewModel> PokemonWithAbility
    {
        get
        {
            var list = new List<PokemonMiniViewModel>();
            foreach (var id in _pokemonService.ValidIds())
            {
                var pokemon = _pokemonService.Retrieve(id);
                if (pokemon.Ability1 == _id || pokemon.Ability2 == _id || pokemon.Ability3 == _id)
                {
                    list.Add(new PokemonMiniViewModel(_cachedSpriteProvider, pokemon, id, _selectPokemonCommand));
                }
            }
            return list;
        }
    }
}