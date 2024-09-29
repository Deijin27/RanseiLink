using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class AbilityViewModel : ViewModelBase
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

    public void SetModel(AbilityId id, Ability model)
    {
        _id = id;
        Id = (int)id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityDescription, Id, value);
    }

    public string HotSpringsDescription
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityHotSpringsDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityHotSpringsDescription, Id, value);
    }

    public string HotSpringsDescription2
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityHotSpringsDescription2, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityHotSpringsDescription2, Id, value);
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