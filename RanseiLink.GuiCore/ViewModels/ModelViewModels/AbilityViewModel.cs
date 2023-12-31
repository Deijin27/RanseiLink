#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public class AbilityViewModel : ViewModelBase
{
    private Ability _model;
    private readonly ICachedMsgBlockService _msgService;
    private readonly IPokemonService _pokemonService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private AbilityId _id;

    public AbilityViewModel(
        ICachedMsgBlockService msgService,
        IJumpService jumpService,
        IPokemonService pokemonService,
        ICachedSpriteProvider cachedSpriteProvider)
    {
        _pokemonService = pokemonService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _msgService = msgService;
        _model = new Ability();

        _selectPokemonCommand = new RelayCommand<PokemonMiniViewModel>(pk => { if (pk != null) jumpService.JumpTo(PokemonSelectorEditorModule.Id, pk.Id); });
    }

    public void SetModel(AbilityId id, Ability model)
    {
        _id = id;
        Id = (int)id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public AbilityEffectId Effect1
    {
        get => _model.Effect1;
        set => RaiseAndSetIfChanged(_model.Effect1, value, v => _model.Effect1 = v);
    }

    public int Effect1Amount
    {
        get => _model.Effect1Amount;
        set => RaiseAndSetIfChanged(_model.Effect1Amount, value, v => _model.Effect1Amount = value);
    }

    public AbilityEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public int Effect2Amount
    {
        get => _model.Effect2Amount;
        set => RaiseAndSetIfChanged(_model.Effect2Amount, value, v => _model.Effect2Amount = value);
    }

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